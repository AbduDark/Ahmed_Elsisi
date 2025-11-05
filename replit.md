# Line Management System for Telecom Providers

## Overview
This project is a Windows desktop application (WPF) designed to manage mobile phone lines for Egyptian telecom providers (Vodafone, Etisalat, WE, Orange). Its primary purpose is to organize groups of lines, track automatic renewals, manage customer deliveries, and support barcode scanning for efficient data entry. The system aims to streamline line management operations for telecom agents.

## User Preferences
Preferred communication style: Simple, everyday language.

## System Architecture

### Frontend
- **Framework**: WPF (.NET desktop UI framework)
- **Platform**: Windows-only (Windows 10 or newer)
- **Language**: C# with XAML
- **UI Design**: Color-coded interfaces per telecom provider (Vodafone=Red, Etisalat=Green, WE=Purple, Orange=Orange). Utilizes MaterialDesignThemes for UI components and supports light/dark modes.

### Core Business Logic
- **Group Management**: Supports up to 50 lines per group across four telecom providers. Lines have states (Active, Suspended, Blocked, With/Without Cash Wallet). Groups track responsible employee, customer, and expected delivery date. A "Business Group" type includes confirmation tracking (0-3 levels per line).
- **Line Entity**: Stores person name, national ID (14 digits), phone number, internal ID, cash wallet flag (with optional wallet number), and details field.
- **Automatic Renewal**: Tracks 60-day renewal cycles for cash wallet groups with 7-day and expiration day alerts.
- **Delivery Tracking**: Manages customer assignment and expected delivery dates with 3-day pre-delivery and overdue alerts.
- **Backup & Restore**: Manual and automatic (24-hour) backup system with restore functionality and cleanup of old backups.
- **Reporting**: Export functionality for lines and groups to Excel and PDF formats, including provider-specific branding and statistics.
- **Excel Import**: Smart import from Excel files with intelligent column detection supporting Arabic/English headers (with or without headers). Auto-validates national IDs (14 digits) and phone numbers (11 digits starting with 01). Internal ID generated from row number.

### Data Architecture
- **Relationships**: Groups can have many Lines (1:Many).
- **Validation**: National ID (14-digit format) and conditional wallet number.

### Input Methods
- **Hardware Integration**: Supports USB 1D barcode scanners (e.g., XB-2055) for rapid line data entry.
- **Workflow**: Optimized data entry with auto-field navigation and auto-save on Enter key press.

### System Design
- **MVVM Pattern**: Utilizes Model-View-ViewModel for separation of concerns.
- **Theming**: Comprehensive light and dark mode support across all windows with dynamic resource binding.
- **Error Handling**: Improved delete confirmation and error messages.

## External Dependencies

### Hardware
- **XB-2055 USB 1D Barcode Scanner**: For barcode/QR code scanning.

### Platform Requirements
- **Windows Operating System**: Windows 10 or newer.
- **.NET Framework/Runtime**: .NET 7.0 for WPF applications.

### Data Storage
- **Database**: SQLite (local `linemanagement.db` file).
- **ORM**: Entity Framework Core 7.0.
- **Entities**: LineGroup, PhoneLine, Alert.

### Third-Party Packages
- **Microsoft.EntityFrameworkCore.Sqlite (7.0.20)**: ORM for SQLite.
- **Newtonsoft.Json (13.0.3)**: JSON serialization.
- **MaterialDesignThemes (4.9.0)**: Material Design UI components.
- **ClosedXML (0.102.1)**: Excel import/export functionality.
- **QuestPDF (2024.3.0)**: PDF report generation.

## Recent Changes (November 2025)

### Bug Fixes
- **NullReferenceException Fix**: Fixed crash when deleting groups or lines by storing group name before LoadGroups() call.

### New Features
- **Excel Import System**: 
  - Added ImportService with smart column detection algorithm
  - Supports files with or without headers
  - Detects Arabic and English column names (اسم, Name, الرقم القومي, National ID, رقم, Phone, Mobile, etc.)
  - Auto-validates national IDs (14 digits) and phone numbers (11 digits starting with 01)
  - Normalizes phone numbers (removes spaces, dashes, +2 prefix)
  - Generates Internal ID from row number
  - Provides detailed import results with error reporting
  - Added "📥 استيراد من Excel" button in GroupDetailsWindow
  - GroupService.ImportLines() validates max lines limit before importing