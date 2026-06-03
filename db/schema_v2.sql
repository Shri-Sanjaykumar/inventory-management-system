-- ============================================================================
-- SQL Server Database Schema v2 (Inventory Management System)
-- ============================================================================

USE [master];
GO

-- Create database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UserManagementDB')
BEGIN
    CREATE DATABASE [UserManagementDB];
END
GO

USE [UserManagementDB];
GO

-- 1. DROP Tables in order of foreign key dependencies if they exist
IF OBJECT_ID(N'dbo.StockReceipts', N'U') IS NOT NULL DROP TABLE dbo.StockReceipts;
IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL DROP TABLE dbo.Items;
IF OBJECT_ID(N'dbo.Projects', N'U') IS NOT NULL DROP TABLE dbo.Projects;
IF OBJECT_ID(N'dbo.Vendors', N'U') IS NOT NULL DROP TABLE dbo.Vendors;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- 2. Create Users Table (Authentication)
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Designation NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Users_Status DEFAULT N'Active',

    CONSTRAINT PK_Users_UserId PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT CK_Users_Status CHECK (Status IN (N'Active', N'Inactive'))
);

-- 3. Create Vendors Table (Vendor Master)
CREATE TABLE dbo.Vendors (
    VendorID INT IDENTITY(1,1) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    AddressLine1 NVARCHAR(250) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    Pincode NVARCHAR(20) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL,
    CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_Vendors_CreatedOn DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Vendors_VendorID PRIMARY KEY CLUSTERED (VendorID),
    CONSTRAINT CK_Vendors_FirstName_NotEmpty CHECK (LEN(TRIM(FirstName)) > 0),
    CONSTRAINT CK_Vendors_LastName_NotEmpty CHECK (LEN(TRIM(LastName)) > 0)
);

-- 4. Create Projects Table (Project Master)
CREATE TABLE dbo.Projects (
    ProjectID INT IDENTITY(1,1) NOT NULL,
    ProjectName NVARCHAR(150) NOT NULL,
    AddressLine1 NVARCHAR(250) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    Pincode NVARCHAR(20) NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL,
    CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_Projects_CreatedOn DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Projects_ProjectID PRIMARY KEY CLUSTERED (ProjectID),
    CONSTRAINT CK_Projects_ProjectName_NotEmpty CHECK (LEN(TRIM(ProjectName)) > 0)
);

-- 5. Create Items Table (Item Master)
CREATE TABLE dbo.Items (
    ItemID INT IDENTITY(1,1) NOT NULL,
    ItemName NVARCHAR(150) NOT NULL,
    UnitOfMeasure NVARCHAR(50) NOT NULL,
    OpeningBalance INT NOT NULL CONSTRAINT DF_Items_OpeningBalance DEFAULT 0,
    DetailedDescription NVARCHAR(MAX) NULL,

    CONSTRAINT PK_Items_ItemID PRIMARY KEY CLUSTERED (ItemID),
    CONSTRAINT CK_Items_ItemName_NotEmpty CHECK (LEN(TRIM(ItemName)) > 0),
    CONSTRAINT CK_Items_OpeningBalance_NonNegative CHECK (OpeningBalance >= 0)
);

-- 6. Create Stock Receipts Table (Stock Receipt Module)
CREATE TABLE dbo.StockReceipts (
    StockReceiptID INT IDENTITY(1,1) NOT NULL,
    VendorID INT NOT NULL,
    ProjectID INT NOT NULL,
    ItemID INT NOT NULL,
    ReceiptDate DATETIME2 NOT NULL,
    Quantity INT NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL,
    CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_StockReceipts_CreatedOn DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_StockReceipts_StockReceiptID PRIMARY KEY CLUSTERED (StockReceiptID),
    CONSTRAINT FK_StockReceipts_Vendors_VendorID FOREIGN KEY (VendorID) REFERENCES dbo.Vendors (VendorID) ON DELETE NO ACTION,
    CONSTRAINT FK_StockReceipts_Projects_ProjectID FOREIGN KEY (ProjectID) REFERENCES dbo.Projects (ProjectID) ON DELETE NO ACTION,
    CONSTRAINT FK_StockReceipts_Items_ItemID FOREIGN KEY (ItemID) REFERENCES dbo.Items (ItemID) ON DELETE NO ACTION,
    CONSTRAINT CK_StockReceipts_Quantity_Positive CHECK (Quantity > 0)
);
GO

-- 7. Indexes for Query Performance Optimization
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON dbo.Users(Username) INCLUDE(PasswordHash, FullName, Status);
CREATE NONCLUSTERED INDEX IX_Vendors_FirstName ON dbo.Vendors(FirstName) INCLUDE(LastName, Email, PhoneNumber);
CREATE NONCLUSTERED INDEX IX_Projects_ProjectName ON dbo.Projects(ProjectName);
CREATE NONCLUSTERED INDEX IX_Items_ItemName ON dbo.Items(ItemName);
CREATE NONCLUSTERED INDEX IX_StockReceipts_ReceiptDate ON dbo.StockReceipts(ReceiptDate);
GO
