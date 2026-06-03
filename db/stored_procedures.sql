-- ============================================================================
-- SQL Server Stored Procedures for User Management
-- ============================================================================

-- Procedure: Register User
CREATE PROCEDURE dbo.usp_User_Register
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(150),
    @Designation NVARCHAR(100),
    @Status NVARCHAR(20) = N'Active',
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewUserId = 0;

    BEGIN TRY
        -- Check constraints before insert
        IF @Username IS NULL OR LEN(TRIM(@Username)) = 0 THROW 50001, N'Username cannot be empty.', 1;
        IF @PasswordHash IS NULL OR LEN(TRIM(@PasswordHash)) = 0 THROW 50002, N'Password hash cannot be empty.', 1;
        IF @FullName IS NULL OR LEN(TRIM(@FullName)) = 0 THROW 50003, N'Full name cannot be empty.', 1;
        IF @Designation IS NULL OR LEN(TRIM(@Designation)) = 0 THROW 50004, N'Designation cannot be empty.', 1;
        
        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
        BEGIN
            THROW 50006, N'Username is already registered.', 1;
        END

        BEGIN TRANSACTION;
            INSERT INTO dbo.Users (Username, PasswordHash, FullName, Designation, Status)
            VALUES (TRIM(@Username), @PasswordHash, TRIM(@FullName), TRIM(@Designation), @Status);
            
            SET @NewUserId = SCOPE_IDENTITY();
        COMMIT TRANSACTION;

        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Procedure: Get User Details By Username
CREATE PROCEDURE dbo.usp_User_GetByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, PasswordHash, FullName, Designation, CreatedDate, Status
    FROM dbo.Users
    WHERE Username = TRIM(@Username);
END
GO

-- Procedure: Update User Status
CREATE PROCEDURE dbo.usp_User_UpdateStatus
    @UserId INT,
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Status NOT IN (N'Active', N'Inactive') THROW 50005, N'Invalid status.', 1;
        
        UPDATE dbo.Users
        SET Status = @Status
        WHERE UserId = @UserId;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
