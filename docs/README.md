# Training Center Management System

## Overview
A RESTful backend API for managing a training center, including users, students, instructors, courses, and enrollments.

## Planned Features
- User management
- Student and instructor management
- Course management and instructor assignments
- Student course enrollment
- Secure authentication and authorization
- Input validation and centralized error handling
- API documentation using Swagger / OpenAPI

## Technologies
- C# and ASP.NET Core Web API (.NET 8)
- SQL Server and T-SQL
- ADO.NET and Entity Framework Core
- Git and GitHub
- Swagger / OpenAPI

## Database Design
The database design and implementation are completed.

- [Entity Relationship Diagram](database/ERD.png)
- [Relational Schema](database/RS.png)

## Project Structure
The solution follows a layered architecture:

- **APIs:** HTTP endpoints and request/response handling.
- **Business:** Business logic and application services.
- **DataAccess:** Database operations using ADO.NET and Entity Framework Core.
- **Domain:** Entities, DTOs, and shared application models.

## Project Status

**Status: In Progress**

### Completed
- Database design and implementation.
- Solution structure and project dependencies.
- Database connection configuration and dependency injection.
- Centralized exception handling and error logging.
- People management endpoints.
- Student management endpoints.
- instructor management endpoints.
- Course management and instructor assignments.
- Student enrollment management.

### In Progress
- User management.
- Authentication and authorization.

### Planned
- API documentation and usage examples.

## Future Documentation
Setup instructions, API endpoints, authentication details, and usage examples will be documented as development progresses.
