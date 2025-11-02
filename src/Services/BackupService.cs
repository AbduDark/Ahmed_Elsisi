using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.Services
{
    public class BackupService
    {
        private readonly string _backupDirectory;
        private System.Timers.Timer? _autoBackupTimer;

        public BackupService()
        {
            _backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        public async Task<string> CreateBackup(string? customName = null)
        {
            try
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "linemanagement.db");
                if (!File.Exists(dbPath))
                {
                    throw new FileNotFoundException("قاعدة البيانات غير موجودة");
                }

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupFileName = customName != null 
                    ? $"{customName}_{timestamp}.db" 
                    : $"backup_{timestamp}.db";
                var backupPath = Path.Combine(_backupDirectory, backupFileName);

                await Task.Run(() => File.Copy(dbPath, backupPath, overwrite: false));

                CleanOldBackups();

                return backupPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"فشل إنشاء النسخة الاحتياطية: {ex.Message}");
            }
        }

        public async Task RestoreBackup(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    throw new FileNotFoundException("النسخة الاحتياطية المحددة غير موجودة");
                }

                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "linemanagement.db");
                var tempBackupPath = dbPath + ".temp_backup";

                if (File.Exists(dbPath))
                {
                    await Task.Run(() => File.Copy(dbPath, tempBackupPath, overwrite: true));
                }

                try
                {
                    await Task.Run(() => File.Copy(backupPath, dbPath, overwrite: true));
                }
                catch
                {
                    if (File.Exists(tempBackupPath))
                    {
                        await Task.Run(() => File.Copy(tempBackupPath, dbPath, overwrite: true));
                    }
                    throw;
                }
                finally
                {
                    if (File.Exists(tempBackupPath))
                    {
                        File.Delete(tempBackupPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"فشل استعادة النسخة الاحتياطية: {ex.Message}");
            }
        }

        public string[] GetBackupList()
        {
            if (!Directory.Exists(_backupDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(_backupDirectory, "*.db")
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .ToArray();
        }

        public void DeleteBackup(string backupPath)
        {
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }

        private void CleanOldBackups(int maxBackups = 50)
        {
            var backups = GetBackupList();
            if (backups.Length > maxBackups)
            {
                foreach (var oldBackup in backups.Skip(maxBackups))
                {
                    try
                    {
                        File.Delete(oldBackup);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void StartAutoBackup(int intervalHours = 24)
        {
            StopAutoBackup();

            _autoBackupTimer = new System.Timers.Timer(TimeSpan.FromHours(intervalHours).TotalMilliseconds);
            _autoBackupTimer.Elapsed += async (sender, e) =>
            {
                try
                {
                    await CreateBackup("auto");
                }
                catch
                {
                }
            };
            _autoBackupTimer.Start();

            _ = CreateBackup("auto_initial");
        }

        public void StopAutoBackup()
        {
            if (_autoBackupTimer != null)
            {
                _autoBackupTimer.Stop();
                _autoBackupTimer.Dispose();
                _autoBackupTimer = null;
            }
        }

        public bool IsAutoBackupEnabled => _autoBackupTimer != null && _autoBackupTimer.Enabled;

        public string BackupDirectory => _backupDirectory;
    }
}
