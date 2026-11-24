# Training Center Management System

## Overview

A RESTful backend API for managing a training center, including users, students, instructors, courses, and enrollments.

## Features

- User management
- Student and instructor management
- Course management and instructor assignments
- Student course enrollment
- Authentication and authorization using JWT
- Refresh token management
- HTTPS and CORS configuration
- Rate limiting
- Input validation
- Centralized exception handling and error logging
- API documentation using Swagger / OpenAPI

## Technologies

- C# and ASP.NET Core Web API (.NET 8)
- SQL Server and T-SQL
- ADO.NET and Entity Framework Core
- Git and GitHub
- Swagger / OpenAPI

## Project Structure

The solution follows a layered architecture:

- **APIs:** HTTP endpoints and request/response handling.
- **Business:** Business logic and application services.
- **DataAccess:** Database operations using ADO.NET and Entity Framework Core.
- **Domain:** Shared entities, DTOs, enums, and application models.

## Getting Started

### 1. Prerequisites

Install the following:

- Visual Studio 2022
- .NET 8 SDK
- SQL Server (Database Engine)
- SQL Server Management Studio (SSMS)

During Visual Studio installation, make sure to select the **ASP.NET and web development** workload.

No additional Visual Studio workloads are required for this project.

### 2. Database Setup

1. Open SQL Server Management Studio and connect to your SQL Server instance.
2. Run the database creation script and the stored procedures script in docs/database.

### 3. Configure User Secrets

Open a terminal in the `solution/APIs` directory.

Configure the database connection string using User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:YOUR_CONNECTION_NAME" "Server=YOUR_SERVER_NAME;Database=TrainingCenter;Trusted_Connection=True;TrustServerCertificate=True;"
```

Replace `YOUR_CONNECTION_NAME` with the connection string key used by the application, and `YOUR_SERVER_NAME` with your SQL Server instance name.

Configure the JWT signing key:

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_KEY"
```

Generate your own cryptographically random key of at least 32 bytes (256 bits). Do not use the example placeholder as an actual key.

### 4. HTTPS Development Certificate

Run the following command in a terminal:

```bash
dotnet dev-certs https --trust
```

This trusts the ASP.NET Core development HTTPS certificate on your machine.

### 5. Configure Local URLs and CORS

Before running the application, update the local URLs to match your environment.

- Update the allowed origins in the CORS configuration in `Program.cs`.
- Update the application URLs in `launchSettings.json`.

Make sure the URLs and ports match your local setup, including the HTTP or HTTPS scheme.

### 6. Open and Run the Solution

1. Open `solution/solution.sln` in Visual Studio.
2. Allow Visual Studio to restore the NuGet packages.
3. Build the solution.
4. Run the APIs project.

If NuGet packages are not restored automatically, restore them through Visual Studio or the .NET CLI.

### 7. Create the Initial Admin Account

The registration endpoint assigns the `user` role by default.

To create the initial administrator account:

1. Open `solution/Business/AuthService.cs` in Visual Studio.
2. Locate the registration method (`RegisterAsync`) where the new user's role is assigned.
3. Temporarily change the assigned role from `user` to `admin`.
4. Run the application and open Swagger.
5. Register the account that you want to use as the initial administrator.
6. Close Swagger and stop the application.
7. Restore the assigned role to `user`.
8. Run the application again and log in using the administrator account you created.

This is a one-time setup procedure. New registrations will receive the regular `user` role.

## Database Design

The database design and implementation are completed.

The ERD and Relational Schema diagrams were intentionally removed from the repository. The SQL database creation and stored procedures scripts are provided instead.

## Project Status

**Status: Finished**

### Completed

- Database implementation and SQL scripts.
- Solution structure and project dependencies.
- Database connection configuration and dependency injection.
- Centralized exception handling and error logging.
- People management endpoints.
- Student management endpoints.
- Instructor management endpoints.
- Course management and instructor assignments.
- Student enrollment management.
- User management.
- Authentication and authorization.
- Refresh token management.
- HTTPS and CORS configuration.
- Rate limiting.
- Swagger / OpenAPI documentation.