-- ============================================================================
-- SQL Server Raw Verification Queries
-- ============================================================================

-- 1. Check if Username is unique
SELECT COUNT(1) FROM dbo.Users WHERE Username = N'johndoe';

-- 2. Insert new user directly (Alternative to Stored Procedure)
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Designation, Status)
VALUES (N'johndoe', N'$2a$10$eImiTXAkRy9f6b4H.H7b7O8f7oP.k434.J2sY5O6e6cK3.N.m0H2C', N'John Doe', N'Developer', N'Active');

-- 3. Retrieve user profile details for login check
SELECT UserId, Username, PasswordHash, FullName, Designation, Status
FROM dbo.Users
WHERE Username = N'johndoe' AND Status = N'Active';

-- 4. Select all registered users (Audit log)
SELECT UserId, Username, FullName, Designation, CreatedDate, Status
FROM dbo.Users
ORDER BY CreatedDate DESC;
