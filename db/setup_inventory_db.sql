-- ============================================================================
-- SQL Server Unified Database Setup Script (Schema + Seed Data)
-- ============================================================================

USE [master];
GO

-- 1. Create database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UserManagementDB')
BEGIN
    CREATE DATABASE [UserManagementDB];
    PRINT 'SUCCESS: Database UserManagementDB created.';
END
ELSE
BEGIN
    PRINT 'INFO: Database UserManagementDB already exists.';
END
GO

USE [UserManagementDB];
GO

-- 2. Drop existing tables in order of foreign keys to avoid dependency conflicts
IF OBJECT_ID(N'dbo.StockReceipts', N'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.StockReceipts;
    PRINT 'SUCCESS: Dropped existing table dbo.StockReceipts';
END

IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Items;
    PRINT 'SUCCESS: Dropped existing table dbo.Items';
END

IF OBJECT_ID(N'dbo.Projects', N'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Projects;
    PRINT 'SUCCESS: Dropped existing table dbo.Projects';
END

IF OBJECT_ID(N'dbo.Vendors', N'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Vendors;
    PRINT 'SUCCESS: Dropped existing table dbo.Vendors';
END

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Users;
    PRINT 'SUCCESS: Dropped existing table dbo.Users';
END
GO

-- 3. Create Users Table (Authentication)
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
PRINT 'SUCCESS: Created table dbo.Users';

-- 4. Create Vendors Table (Vendor Master)
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
PRINT 'SUCCESS: Created table dbo.Vendors';

-- 5. Create Projects Table (Project Master)
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
PRINT 'SUCCESS: Created table dbo.Projects';

-- 6. Create Items Table (Item Master)
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
PRINT 'SUCCESS: Created table dbo.Items';

-- 7. Create Stock Receipts Table (Stock Receipt Module)
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
PRINT 'SUCCESS: Created table dbo.StockReceipts';
GO

-- 8. Indexes for Query Performance Optimization
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON dbo.Users(Username) INCLUDE(PasswordHash, FullName, Status);
CREATE NONCLUSTERED INDEX IX_Vendors_FirstName ON dbo.Vendors(FirstName) INCLUDE(LastName, Email, PhoneNumber);
CREATE NONCLUSTERED INDEX IX_Projects_ProjectName ON dbo.Projects(ProjectName);
CREATE NONCLUSTERED INDEX IX_Items_ItemName ON dbo.Items(ItemName);
CREATE NONCLUSTERED INDEX IX_StockReceipts_ReceiptDate ON dbo.StockReceipts(ReceiptDate);
PRINT 'SUCCESS: Created indexes.';
GO

-- ============================================================================
-- Seed Testing Data
-- ============================================================================

-- Seed Users (password: Password123!)
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Designation, Status)
VALUES (
    N'sanjay326', 
    N'$2a$11$lyLOPgkfol6GbRw7gMX9.OInmsR6783Z.pQlkOGgq0hHdryVyzGSC', 
    N'Shri Sanjaykumar V', 
    N'Backend Developer', 
    N'Active'
);

-- Seed Vendors
INSERT INTO dbo.Vendors (FirstName, LastName, AddressLine1, City, State, Pincode, Email, PhoneNumber, CreatedBy)
VALUES 
(N'Suresh', N'Kumar', N'123 Industrial Area', N'Chennai', N'Tamil Nadu', N'600001', N'suresh.k@sureshmetals.com', N'+91 98765 43210', N'sanjay326'),
(N'Amit', N'Sharma', N'45 Commercial St', N'Mumbai', N'Maharashtra', N'400001', N'info@sharmasteel.com', N'+91 91234 56789', N'sanjay326'),
(N'Deepak', N'Gupta', N'78 GIDC Sector 2', N'Ahmedabad', N'Gujarat', N'380001', N'deepak@guptatraders.com', N'+91 88888 77777', N'sanjay326'),
(N'Ananya', N'Patel', N'12 Business Hub', N'Bangalore', N'Karnataka', N'560001', N'sales@patelpipes.com', N'+91 77777 66666', N'sanjay326');

-- Seed Projects
INSERT INTO dbo.Projects (ProjectName, AddressLine1, City, State, Pincode, CreatedBy)
VALUES 
(N'Metro Rail Extension Phase 3', N'Mount Road Construction Site', N'Chennai', N'Tamil Nadu', N'600002', N'sanjay326'),
(N'Smart City Smart Water Grid', N'Sector 5 Water Plant', N'Mumbai', N'Maharashtra', N'400015', N'sanjay326'),
(N'Greenfield IT Park Phase 1', N'Electronic City Phase 2', N'Bangalore', N'Karnataka', N'560100', N'sanjay326');

-- Seed Items
INSERT INTO dbo.Items (ItemName, UnitOfMeasure, OpeningBalance, DetailedDescription)
VALUES 
(N'Reinforced Steel Rebars 12mm', N'Metric Tons', 150, N'TMT high-strength reinforced steel rebars for concrete reinforcing structural projects.'),
(N'PVC Pressure Pipes 4 Inch', N'Meters', 500, N'Class 3 heavy-duty PVC pipes for municipal water transmission networks.'),
(N'Portland Cement Grade 53', N'Bags', 1200, N'High-strength Portland cement for structural slabs, pillars, and flyover works.');

PRINT 'SUCCESS: Seed data inserted.';
GO
