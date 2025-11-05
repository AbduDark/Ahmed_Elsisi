using ClosedXML.Excel;
using LineManagementSystem.Models;
using System.Text.RegularExpressions;

namespace LineManagementSystem.Services;

public class ImportService
{
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<PhoneLine> ImportedLines { get; set; } = new();
    }

    public class ColumnMapping
    {
        public int NameColumn { get; set; }
        public int NationalIdColumn { get; set; }
        public int PhoneNumberColumn { get; set; }
    }

    public ImportResult ImportFromExcel(string filePath, int groupId)
    {
        var result = new ImportResult();

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add("الملف لا يحتوي على أي أوراق عمل");
                return result;
            }

            var mapping = DetectColumns(worksheet);
            
            if (mapping == null)
            {
                result.Errors.Add("لم يتم العثور على الأعمدة المطلوبة (الاسم، الرقم القومي، رقم الخط)");
                return result;
            }

            var firstDataRow = 2;
            var hasHeader = HasHeaderRow(worksheet);
            if (!hasHeader)
            {
                firstDataRow = 1;
            }

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = firstDataRow; row <= lastRow; row++)
            {
                try
                {
                    var name = worksheet.Cell(row, mapping.NameColumn).GetString().Trim();
                    var nationalId = worksheet.Cell(row, mapping.NationalIdColumn).GetString().Trim();
                    var phoneNumber = worksheet.Cell(row, mapping.PhoneNumberColumn).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(name) || 
                        string.IsNullOrWhiteSpace(nationalId) || 
                        string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        continue;
                    }

                    phoneNumber = NormalizePhoneNumber(phoneNumber);
                    nationalId = NormalizeNationalId(nationalId);

                    if (!IsValidNationalId(nationalId))
                    {
                        result.Errors.Add($"صف {row}: رقم قومي غير صحيح - {nationalId}");
                        result.FailedCount++;
                        continue;
                    }

                    if (!IsValidPhoneNumber(phoneNumber))
                    {
                        result.Errors.Add($"صف {row}: رقم خط غير صحيح - {phoneNumber}");
                        result.FailedCount++;
                        continue;
                    }

                    var internalId = (row - (hasHeader ? 1 : 0)).ToString();

                    var phoneLine = new PhoneLine
                    {
                        Name = name,
                        NationalId = nationalId,
                        PhoneNumber = phoneNumber,
                        InternalId = internalId,
                        GroupId = groupId
                    };

                    result.ImportedLines.Add(phoneLine);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"صف {row}: {ex.Message}");
                    result.FailedCount++;
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"خطأ في قراءة الملف: {ex.Message}");
        }

        return result;
    }

    private ColumnMapping? DetectColumns(IXLWorksheet worksheet)
    {
        var firstRow = 1;
        var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        if (lastColumn < 3)
            return null;

        var hasHeader = HasHeaderRow(worksheet);
        
        if (hasHeader)
        {
            return DetectColumnsFromHeader(worksheet, lastColumn);
        }
        else
        {
            return DetectColumnsFromData(worksheet, lastColumn);
        }
    }

    private bool HasHeaderRow(IXLWorksheet worksheet)
    {
        var firstRow = worksheet.Row(1);
        var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;
        
        int textCellCount = 0;
        int totalCells = 0;

        for (int col = 1; col <= Math.Min(lastColumn, 10); col++)
        {
            var cellValue = firstRow.Cell(col).GetString();
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                totalCells++;
                if (!IsNumericOnly(cellValue))
                {
                    textCellCount++;
                }
            }
        }

        return totalCells > 0 && textCellCount >= totalCells / 2;
    }

    private ColumnMapping? DetectColumnsFromHeader(IXLWorksheet worksheet, int lastColumn)
    {
        var mapping = new ColumnMapping();
        var headerRow = worksheet.Row(1);

        for (int col = 1; col <= lastColumn; col++)
        {
            var header = headerRow.Cell(col).GetString().ToLower().Trim();
            
            if (header.Contains("اسم") || header.Contains("name") || header.Contains("الاسم"))
            {
                mapping.NameColumn = col;
            }
            else if (header.Contains("قومي") || header.Contains("national") || header.Contains("رقم قومي") || header.Contains("الرقم القومي"))
            {
                mapping.NationalIdColumn = col;
            }
            else if (header.Contains("رقم") && !header.Contains("قومي") || header.Contains("phone") || header.Contains("خط") || header.Contains("الرقم"))
            {
                mapping.PhoneNumberColumn = col;
            }
        }

        if (mapping.NameColumn > 0 && mapping.NationalIdColumn > 0 && mapping.PhoneNumberColumn > 0)
        {
            return mapping;
        }

        return null;
    }

    private ColumnMapping? DetectColumnsFromData(IXLWorksheet worksheet, int lastColumn)
    {
        var mapping = new ColumnMapping();
        var sampleSize = Math.Min(5, worksheet.LastRowUsed()?.RowNumber() ?? 0);

        var columnScores = new Dictionary<int, Dictionary<string, int>>();
        
        for (int col = 1; col <= lastColumn; col++)
        {
            columnScores[col] = new Dictionary<string, int>
            {
                ["name"] = 0,
                ["nationalId"] = 0,
                ["phone"] = 0
            };
        }

        for (int row = 1; row <= sampleSize; row++)
        {
            for (int col = 1; col <= lastColumn; col++)
            {
                var value = worksheet.Cell(row, col).GetString().Trim();
                
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var cleanValue = value.Replace(" ", "").Replace("-", "");

                if (IsArabicOrEnglishText(value) && !IsNumericOnly(cleanValue))
                {
                    columnScores[col]["name"] += 10;
                }

                if (cleanValue.Length == 14 && IsNumericOnly(cleanValue))
                {
                    columnScores[col]["nationalId"] += 20;
                }

                if (cleanValue.Length == 11 && IsNumericOnly(cleanValue) && cleanValue.StartsWith("01"))
                {
                    columnScores[col]["phone"] += 20;
                }
            }
        }

        var nameCol = columnScores.OrderByDescending(x => x.Value["name"]).FirstOrDefault();
        var nationalIdCol = columnScores.OrderByDescending(x => x.Value["nationalId"]).FirstOrDefault();
        var phoneCol = columnScores.OrderByDescending(x => x.Value["phone"]).FirstOrDefault();

        if (nameCol.Value["name"] > 0 && nationalIdCol.Value["nationalId"] > 0 && phoneCol.Value["phone"] > 0)
        {
            mapping.NameColumn = nameCol.Key;
            mapping.NationalIdColumn = nationalIdCol.Key;
            mapping.PhoneNumberColumn = phoneCol.Key;
            return mapping;
        }

        return null;
    }

    private bool IsArabicOrEnglishText(string text)
    {
        return Regex.IsMatch(text, @"[\u0600-\u06FFa-zA-Z]");
    }

    private bool IsNumericOnly(string text)
    {
        return Regex.IsMatch(text.Replace(" ", "").Replace("-", ""), @"^\d+$");
    }

    private string NormalizePhoneNumber(string phoneNumber)
    {
        var cleaned = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        if (cleaned.StartsWith("+2"))
        {
            cleaned = cleaned.Substring(2);
        }
        else if (cleaned.StartsWith("2") && cleaned.Length == 12)
        {
            cleaned = cleaned.Substring(1);
        }

        return cleaned;
    }

    private string NormalizeNationalId(string nationalId)
    {
        return nationalId.Replace(" ", "").Replace("-", "");
    }

    private bool IsValidNationalId(string nationalId)
    {
        return nationalId.Length == 14 && IsNumericOnly(nationalId);
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length == 11 && phoneNumber.StartsWith("01") && IsNumericOnly(phoneNumber);
    }
}
