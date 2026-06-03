-- ============================================================================
-- SQL Server Seed Data Script (Inventory Management System)
-- ============================================================================

USE [UserManagementDB];
GO

-- 1. Insert Initial User (password: Password123!)
-- Hash generated using standard BCrypt algorithm
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'sanjay326')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, FullName, Designation, Status)
    VALUES (
        N'sanjay326', 
        N'$2a$11$lyLOPgkfol6GbRw7gMX9.OInmsR6783Z.pQlkOGgq0hHdryVyzGSC', 
        N'Shri Sanjaykumar V', 
        N'Backend Developer', 
        N'Active'
    );
END
GO

-- 2. Insert Seed Vendors (Vendor Master)
IF (SELECT COUNT(1) FROM dbo.Vendors) = 0
BEGIN
    INSERT INTO dbo.Vendors (FirstName, LastName, AddressLine1, City, State, Pincode, Email, PhoneNumber, CreatedBy)
    VALUES 
    (N'Suresh', N'Kumar', N'123 Industrial Area', N'Chennai', N'Tamil Nadu', N'600001', N'suresh.k@sureshmetals.com', N'+91 98765 43210', N'sanjay326'),
    (N'Amit', N'Sharma', N'45 Commercial St', N'Mumbai', N'Maharashtra', N'400001', N'info@sharmasteel.com', N'+91 91234 56789', N'sanjay326'),
    (N'Deepak', N'Gupta', N'78 GIDC Sector 2', N'Ahmedabad', N'Gujarat', N'380001', N'deepak@guptatraders.com', N'+91 88888 77777', N'sanjay326'),
    (N'Ananya', N'Patel', N'12 Business Hub', N'Bangalore', N'Karnataka', N'560001', N'sales@patelpipes.com', N'+91 77777 66666', N'sanjay326');
END
GO

-- 3. Insert Seed Projects (Project Master)
IF (SELECT COUNT(1) FROM dbo.Projects) = 0
BEGIN
    INSERT INTO dbo.Projects (ProjectName, AddressLine1, City, State, Pincode, CreatedBy)
    VALUES 
    (N'Metro Rail Extension Phase 3', N'Mount Road Construction Site', N'Chennai', N'Tamil Nadu', N'600002', N'sanjay326'),
    (N'Smart City Smart Water Grid', N'Sector 5 Water Plant', N'Mumbai', N'Maharashtra', N'400015', N'sanjay326'),
    (N'Greenfield IT Park Phase 1', N'Electronic City Phase 2', N'Bangalore', N'Karnataka', N'560100', N'sanjay326');
END
GO

-- 4. Insert Seed Items (Item Master)
IF (SELECT COUNT(1) FROM dbo.Items) = 0
BEGIN
    INSERT INTO dbo.Items (ItemName, UnitOfMeasure, OpeningBalance, DetailedDescription)
    VALUES 
    (N'Reinforced Steel Rebars 12mm', N'Metric Tons', 150, N'TMT high-strength reinforced steel rebars for concrete reinforcing structural projects.'),
    (N'PVC Pressure Pipes 4 Inch', N'Meters', 500, N'Class 3 heavy-duty PVC pipes for municipal water transmission networks.'),
    (N'Portland Cement Grade 53', N'Bags', 1200, N'High-strength Portland cement for structural slabs, pillars, and flyover works.');
END
GO
