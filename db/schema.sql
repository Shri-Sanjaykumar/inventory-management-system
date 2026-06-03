-- ============================================================================
-- SQL Server Users Schema
-- ============================================================================

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
    CONSTRAINT CK_Users_Status CHECK (Status IN (N'Active', N'Inactive')),
    CONSTRAINT CK_Users_Username_NotEmpty CHECK (LEN(TRIM(Username)) > 0),
    CONSTRAINT CK_Users_FullName_NotEmpty CHECK (LEN(TRIM(FullName)) > 0),
    CONSTRAINT CK_Users_Designation_NotEmpty CHECK (LEN(TRIM(Designation)) > 0)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username
ON dbo.Users(Username)
INCLUDE(PasswordHash, FullName, Status);
GO
