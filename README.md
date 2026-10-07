# AcxiomCRM

ASP.NET Core MVC + Entity Framework Core + ASP.NET Core Identity CRM implementation based on the supplied AcxiomCRM project specification.

## Stack
- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8
- SQLite (easy local setup; replace provider/connection string if SQL Server is required)
- ASP.NET Core Identity
- Bootstrap 5
- Chart.js

## Run
1. Install .NET 8 SDK.
2. Open this folder in Visual Studio or VS Code.
3. Run `dotnet restore`.
4. Run `dotnet run`.
5. Open the displayed localhost URL.

The database is created automatically on first run.

## Seed accounts
- Admin: `admin@acxiomcrm.local` / `Admin@123`
- Manager: `manager@acxiomcrm.local` / `Manager@123`
- Sales Executive: `sales@acxiomcrm.local` / `Sales@123`

Change these passwords before deployment.

## Implemented
- Login, logout, registration, password hashing and lockout via Identity
- Admin / Manager / SalesExecutive roles
- Role-scoped dashboard
- Customer CRUD and duplicate email/phone validation
- Lead CRUD and status/priority workflow fields
- Opportunity CRUD and amount/probability/close-date business rules
- Follow-up scheduling and completion with date validation
- Activity management
- User and role management
- Audit logging for authentication and CRM actions
- Customer, Lead and Opportunity REST APIs using response projections rather than password/security fields
- Reports and dashboard charts
- Bootstrap responsive UI and unobtrusive client validation
- Anti-forgery tokens on state-changing MVC forms

## Important assignment alignment
The supplied specification explicitly requires ASP.NET Core Identity rather than a custom password table, server-side validation, role-based authorization, audit logging, at least one API controller, dashboard analytics and structured Controllers/Models/Data/Services/ViewModels/Views architecture.
