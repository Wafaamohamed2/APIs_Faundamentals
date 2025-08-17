# APIs Fundamentals – ASP.NET Core Web API:
    > This project is a practice implementation of ASP.NET Core Web API demonstrating modern backend development concepts such as Entity Framework Core, Repository Pattern, Generic Repository Pattern, Unit of Work Pattern, and JWT Authentication.

 
    > it provides complete CRUD operations for the entities:
          - Employee
          - Department
      Additionally, the API implements a minimal authentication system using JWT tokens.



## Features:
   - Build RESTful APIs using ASP.NET Core 8.0.
   - Database access with Entity Framework Core.
   - Generic Repository Pattern for reusable CRUD operations.
   - Unit of Work Pattern for managing multiple repositories in a single transaction.
   - DTOs (Data Transfer Objects) for clean API responses.
   - JWT Authentication for securing endpoints.


## Tech Stack
- **ASP.NET Core 8.0** – Web API Framework
- **Entity Framework Core** – ORM for database access
- **SQL Server** – Relational database
- **JWT (JSON Web Tokens)** – Authentication & Authorization
- **Dependency Injection** – For loose coupling
- **Repository Pattern** – Encapsulated data access logic
- **Unit of Work Pattern** – Single transaction management



## Authentication Workflow (JWT):
   1. User registers or logs in using the authentication endpoints.
   2. API issues a JWT Token and a Refresh Token.
   3. The JWT Token is sent with each request (Authorization: Bearer <token>).
   4. Expired tokens can be renewed using the Refresh Token.
   5. Users can revoke tokens to invalidate access.



## Client Application (WinForms):
    This repository also includes a **Windows Forms application** (`App_Consummer`) that consumes 
    the`APIs_Faundamentals` Web API.


### Workflow:
  1. On application load (`Form1_Load`), the client sends an HTTP GET request to:
     https://localhost:7163/api/Employee
  2. The API returns a JSON list of employees.
  3. The JSON response is deserialized into a list of `EmployeeData` objects.
  4. The data is bound to a `DataGridView` (`DGV_Employees`) for display.



    

