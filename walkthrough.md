# Architectural Walkthrough, Security Audit & Quality Assurance Test Report

We have successfully refactored, secured, and polished the **Inventory Management System**. The application has been built, verified locally against a Microsoft SQL Server Express instance, and pushed to your GitHub repository.

---

## 🎨 Architectural Review

1. **Presentation Layer (Razor Views & ViewModels):**
   * Redesigned using a clean, professional Microsoft-style dashboard layout featuring a white content pane with slate gray accents (`#475569`, `#1e293b`).
   * Designed a responsive left sidebar that collapses on mobile/tablet viewports.
   * Leveraged **ViewModels** to separate UI representation from core database entities, preventing over-posting attacks.
2. **Business Logic Layer (Services):**
   * Enforces business rules (e.g. quantities > 0, receipt dates <= today) on both C# controllers and Razor validation templates.
   * Maps current user claims context automatically to audit columns.
3. **Data Access Layer (Generic Repository & EF Core):**
   * Abstracted all database queries using `IRepository<T>` and custom entity repositories (such as relation eager-loading for Stock Receipts).
   * Mapped database indexes for fast query performance.

---

## 🔒 Role-Based Access Control (RBAC) Implementation

We mapped user roles dynamically using the existing `Designation` field.

### System Mapping Rules:
* **Claims Construction:** The `Designation` field of the authenticated user is bound to the `ClaimTypes.Role` claim on login:
  ```csharp
  new Claim(ClaimTypes.Role, user.Designation)
  ```
* **Controller-Level Guard:** We applied `[Authorize(Roles = "Admin,Administrator,Backend Developer")]` attributes to the `Delete` actions in:
  - `VendorController.cs`
  - `ProjectController.cs`
  - `ItemController.cs`
  - `StockReceiptController.cs`
* **UI/UX Conditional Rendering:** Delete buttons are wrapped in a Razor check (`User.IsInRole("Admin")`), making them invisible to standard Operators and Staff:
  ```html
  @if (User.IsInRole("Admin") || User.IsInRole("Administrator"))
  {
      <form asp-action="Delete" ...>...</form>
  }
  ```

---

## 🧪 Quality Assurance & Test Report

### 1. Functional Test Cases

| ID | Test Scenario | Expected Result | Status |
|---|---|---|---|
| FT-01 | User Registration | Selection of designation (`Admin`, `Operator`, `Staff`) saves profile with hashed password. | **Passed** |
| FT-02 | User Login & Authentication | Correct credentials create session claims cookie; incorrect details trigger invalid error alert. | **Passed** |
| FT-03 | Dashboard Landing Page | Renders metrics cards (Total counts of Vendors, Projects, Items, Receipts) and last 5 recent entries. | **Passed** |
| FT-04 | Vendor Master Creation | Creating vendor automatically captures current login name as `CreatedBy` and UTC time. | **Passed** |
| FT-05 | Alphabetical Sorting | Dropdowns on Stock Receipt entry are sorted alphabetically (Vendors by `FirstName`, Projects by `ProjectName`, Items by `ItemName`). | **Passed** |
| FT-06 | Receipt Date Restriction | Future dates are blocked in the calendar picker and rejected on form submission. | **Passed** |
| FT-07 | Quantity Validation | Entering a quantity <= 0 returns a validation error span: "Quantity must be greater than 0." | **Passed** |

### 2. Security Test Cases

| ID | Test Scenario | Expected Result | Status |
|---|---|---|---|
| ST-01 | SQL Injection Protection | LINQ and parameterized SQL queries sanitize inputs; raw queries are prevented. | **Passed** |
| ST-02 | CSRF Protection | Forms include `[ValidateAntiForgeryToken]` token and validating tags. | **Passed** |
| ST-03 | XSS Protection | Razor engine automatically HTML-encodes all dynamic output properties. | **Passed** |
| ST-04 | Unauthorized Endpoint Access | Accessing dashboard/masters without logging in redirects immediately to `/Account/Login`. | **Passed** |
| ST-05 | RBAC Delete Bypass Attempt | Non-admin user sending manual POST to `Delete` action receives HTTP 403 Forbidden. | **Passed** |

### 3. UI/UX Test Cases

| ID | Test Scenario | Expected Result | Status |
|---|---|---|---|
| UT-01 | Desktop Sidebar View | Sidebar remains pinned on left side (`260px` width) with active link highlighted. | **Passed** |
| UT-02 | Mobile Collapsible Menu | Sidebar collapses on mobile screen; hamburger toggle opens sidebar overlay drawer. | **Passed** |
| UT-03 | Content Layout | Corporate style formatting with cards, white background, and slate accents. | **Passed** |

---

## 📋 Final Review Checklist

* [x] **SOLID Principles:** Handled dependency injection (DI) via interfaces; kept controllers slim and business logic decoupled.
* [x] **Microsoft Coding Standards:** Applied PascalCase naming for methods/properties, asynchronous `async/await` patterns for all database calls, and standard logging.
* [x] **Data Integrity Protection:** Configured `DeleteBehavior.Restrict` on database relations, avoiding database orphan records.
* [x] **Error Fallback:** Custom error view configured in `Views/Shared/Error.cshtml` to handle exceptions gracefully.
* [x] **Code Cleanliness:** Deleted temporary generation scripts, updated configuration parameters, and compiled with 0 errors.
