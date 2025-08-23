## APIs Fundamentals – ASP.NET Core Web API
         - This project is a practice implementation of ASP.NET Core Web API that demonstrates Entity Framework Core, 
         Repository Pattern, Generic Repository Pattern, Unit of Work Pattern, and now JWT Authentication with Role Management.
         - It includes full CRUD operations for Employee and Department entities, along with an authentication system
         supporting user registration, login, role assignment , token refresh, and secure endpoint access.


##  Latest Features & Updates:
   ## 1- Authentication & Security:
   - JWT Authentication with custom token validation and secure token handling
   - Refresh Token System with automatic token renewal and revocation 
   - Role-Based Authorization (Admin, User, HR) with granular permissions
   - Rate Limiting with different policies for authentication, general, and admin operations
   - CORS Security with specific origin, method, and header restrictions
Secure Cookie Manageme

  ## 2- API Endpoints: 
   - User Registration (/api/Account/Register) with validation and automatic role assignment
   - User Login (/api/Account/Login) with JWT token generation and refresh token
   - Token Refresh (/api/Account/RefreshToken) for seamless token renewal
   - Token Revocation (/api/Account/RevokeToken) for secure logout  
   - Role Assignment (/api/Account/AssignRole) with admin-only access
   - User Profile (/api/Account/Profile) to get current user information
   - Employee Management with role-based access control
   - Department Management with comprehensive data transfer objects (DTOs)

 ## 3- Security Enhancements:
   - Authorization Policies for different access levels
   - Input Sanitization and validation
   - Error Handling with secure error responses
   - Logging Integration ready for monitoring
   - Rate Limiting to prevent abuse and DDoS attacks

    

## Tech Stack:
   - ASP.NET Core 8.0 – Web API Framework
   - Entity Framework Core – ORM for database access
   - SQL Server – Relational database
   - JWT (JSON Web Tokens) – Authentication
   - ASP.NET Core Identity – User & Role management
   - Serilog – Structured logging (configured)
   - Rate Limiting – Built-in ASP.NET Core rate limiting
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
   2. Login → Response includes: (JWT Access Token , Refresh Token (HttpOnly Cookie) ,User information and roles).
   3. Token Refresh (Automatic): Uses refresh token from secure cookie to get new access token.
   4. Assign Role(Admin Only) → Admin can assign new roles to users.
   5. Secure API Access → Token must be provided in the Authorization header.


 ##  Security Features:

   ## 1. Rate Limiting Policies:
   - Authentication Policy: 10 requests/minute for login/register
   - General Policy: 100 requests/minute for regular operations
   - Admin Policy: 20 requests/minute for admin operations
   - Global Limiter: 200 requests/minute per user

   ## 2. Authorization Policies:
   - AdminOnly: Requires Admin role
   - UserOnly: Requires User role
   - SuperAdminOnly: Requires super_admin claim
   - CanManageEmployees: Admin or HR roles
   - ReadOnlyAccess: Admin, HR, or User roles


 
