# Walkthrough: C# ASP.NET Core MVC Inventory Management System

We have successfully designed and implemented an enterprise-grade **Inventory Management System** using **C#**, **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core (EF Core)**, and **Microsoft SQL Server**.

This system is completely integrated into your workspace folder:
📂 **`C:\Users\shris\work\INTERN-backend`**

---

## 🎨 Professional UI/UX Details (No AI Templates)
- **Minimalist Slate Theme**: Uses a crisp slate-and-white color palette with a clean top navigation bar (`#1e293b`).
- **Responsive Layout**: Designed from scratch using Bootstrap 5 grid layout, adapting seamlessly to mobile, tablet, and desktop viewports.
- **Accurate Form Validations**: Clear error alerts appear below the inputs.
- **Clear Information Hierarchy**: Grid lists present IDs, names, UOM, and audit logging timestamps in a structured, readable manner.

---

## ⚙️ Complete File Structure

```text
C:\Users\shris\work\INTERN-backend\
├── InternInventory.csproj            (C# project file with EF Core & BCrypt packages)
├── appsettings.json                  (SQL Server connection string settings)
├── Program.cs                        (Middleware config, DI registers, cookie auth)
├── walkthrough.md                    (This guide)
├── db/                               (SQL Database Scripts)
│   ├── schema_v2.sql                 (Normalized tables, indexes, constraints)
│   └── seeds_v2.sql                  (Admin user, vendors, items, and projects)
├── Data/
│   └── InventoryDbContext.cs         (EF Core Context mappings)
├── Models/                           (Entity Models with Validations)
│   ├── User.cs                       (User logins)
│   ├── Vendor.cs                     (Vendor fields & phone/email validation)
│   ├── Project.cs                    (Project fields)
│   ├── Item.cs                       (Item fields & non-negative balance check)
│   └── StockReceipt.cs               (StockReceipt fields & foreign keys)
├── ViewModels/                       (Razor view input/output mapping)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── StockReceiptViewModel.cs      (Select-lists for dropdowns)
├── Repositories/                     (Generic & Custom EF Repositories)
│   ├── IRepository.cs / Repository.cs
│   ├── IUserRepository.cs / UserRepository.cs
│   ├── IVendorRepository.cs / VendorRepository.cs
│   ├── IProjectRepository.cs / ProjectRepository.cs
│   ├── IItemRepository.cs / ItemRepository.cs
│   └── IStockReceiptRepository.cs / StockReceiptRepository.cs
├── Services/                         (Service Layer for business rules)
│   ├── IUserService.cs / UserService.cs (BCrypt hash authentication)
│   ├── IVendorService.cs / VendorService.cs (FirstName sorting list)
│   ├── IProjectService.cs / ProjectService.cs (ProjectName sorting list)
│   ├── IItemService.cs / ItemService.cs (ItemName sorting list)
│   └── IStockReceiptService.cs / StockReceiptService.cs (Future date & Qty check)
├── Controllers/                      (MVC Controllers with Cookie Auth)
│   ├── AccountController.cs          (User Login, claims cookie creation, Logout)
│   ├── VendorController.cs           (Vendor Master actions)
│   ├── ProjectController.cs          (Project Master actions)
│   ├── ItemController.cs             (Item Master actions)
│   └── StockReceiptController.cs     (Stock Receipt actions)
└── Views/                            (Bootstrap 5 responsive Razor Views)
    ├── _ViewStart.cshtml / _ViewImports.cshtml
    ├── Shared/
    │   └── _Layout.cshtml            (Main layout sidebar/header & claims)
    ├── Account/
    │   ├── Login.cshtml
    │   └── Register.cshtml
    ├── Vendor/
    │   ├── Index.cshtml / Create.cshtml / Edit.cshtml
    ├── Project/
    │   ├── Index.cshtml / Create.cshtml / Edit.cshtml
    ├── Item/
    │   ├── Index.cshtml / Create.cshtml / Edit.cshtml
    └── StockReceipt/
        ├── Index.cshtml / Create.cshtml / Edit.cshtml
```

---

## 🔒 Implemented Business & Audit Rules

1. **Automatic Audit Tracking**:
   - Creating a Vendor, Project, or Stock Receipt automatically captures the currently logged-in user's username (`CreatedBy`) and the current timestamp (`CreatedOn`).
2. **Dropdown Sorting**:
   - Vendor dropdown options are sorted alphabetically by `FirstName`.
   - Project dropdown options are sorted alphabetically by `ProjectName`.
   - Item dropdown options are sorted alphabetically by `ItemName`.
3. **Strict Quantity Validation**:
   - Stock Receipt quantities must be greater than 0 (`Quantity > 0`).
4. **No Future Dates Allowed**:
   - Receipt date is restricted to today or earlier (`ReceiptDate.Date <= DateTime.Today`).
   - Enforced client-side (via datepicker `max` attribute) and server-side (via Service layer validations).

---

## 🚀 How to Run the Project Locally

### Step 1: Initialize the Database v2
Connect to your local SQL Server (`localhost\SQLEXPRESS`) in SSMS or Azure Data Studio, and execute:
1. First, run: [db/schema_v2.sql](file:///C:/Users/shris/work/INTERN-backend/db/schema_v2.sql)
2. Then, run: [db/seeds_v2.sql](file:///C:/Users/shris/work/INTERN-backend/db/seeds_v2.sql)

### Step 2: Start the Web App
1. Open standard Command Prompt (`cmd`).
2. Navigate to your workspace directory:
   ```cmd
   cd C:\Users\shris\work\INTERN-backend
   ```
3. Run the application:
   ```cmd
   dotnet run
   ```
4. Open your browser and go to:
   👉 **`http://localhost:5000`**

### Step 3: Test and Verify
- **Sign In**: Log in using the credentials from the seed data (Username: `sanjay326`, Password: `Password123!`).
- **Populate Masters**: Go to Vendor, Project, and Item tabs to add details.
- **Record Stock Receipt**: Add a stock receipt. Verify that dropdowns are sorted alphabetically, receipt quantities <= 0 are blocked, and future dates are blocked by the calendar input picker!
