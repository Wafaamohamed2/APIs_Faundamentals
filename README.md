## APIs Fundamentals – ASP.NET Core Web API
         - This project is a practice implementation of ASP.NET Core Web API that demonstrates Entity Framework Core, 
         Repository Pattern, Generic Repository Pattern, Unit of Work Pattern, and now JWT Authentication with Role Management.
         - It includes full CRUD operations for Employee and Department entities, along with an authentication system
         supporting user registration, login, and role assignment.


## Updated Features:
   - User Registration (/api/Account/Register) with validation and automatic role assignment (User role by default).
   - User Login (/api/Account/Login) with JWT token generation.
   - Role Assignment (/api/Account/AssignRole) for assigning roles to existing users.
   - JWT Authentication configuration with custom token validation parameters.
   - Secure API endpoints using [Authorize] attributes based on roles.
   - Extended ApplicationUser model to store additional user details (FirstName, LastName).
   - AuthModel for unified authentication response.
   - TokenReqModel for handling login requests.
   - JWT settings in configuration for issuer, audience, secret key, and token lifetime.

## Tech Stack:
   - ASP.NET Core 8.0 – Web API Framework
   - Entity Framework Core – ORM for database access
   - SQL Server – Relational database
   - JWT (JSON Web Tokens) – Authentication
   - ASP.NET Core Identity – User & Role management
   - Dependency Injection – Loose coupling
   - Repository Pattern – Encapsulated data access logic
   - Unit of Work Pattern – Single transaction management


 ##  Client Application (WinForms):
      The repository also includes a Windows Forms application (App_Consummer) that consumes the updated APIs_Fundamentals Web API.
      
# Workflow:
  1. On application load (Form1_Load), the client sends an HTTP GET request to:  https://localhost:7163/api/Employee
  2. The API returns a JSON list of employees.
  3. The JSON response is deserialized into a list of EmployeeData objects.
  4. The data is bound to a DataGridView (DGV_Employees) for display.

          
 ##  Authentication Flow:
   1. Register → New users are created with User role by default.
   2. Login → Valid credentials return a JWT token containing username, email, and roles.
   3. Assign Role → Admin can assign new roles to users.
   4. Access Protected Endpoints → Token must be provided in the Authorization header.

