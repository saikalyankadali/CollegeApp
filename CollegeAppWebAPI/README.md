# CollegeAppWebAPI

Backend service for CollegeApp — an ASP.NET Core Web API for managing student records, secured with JWT authentication and built on Entity Framework Core.

## Tech Stack

- **.NET** — ASP.NET Core Web API
- **ORM** — Entity Framework Core
- **Database** — SQL Server (CollegeDB)
- **Security** — JWT Bearer authentication, role-based authorization
- **Mapping** — AutoMapper
- **Patterns** — Repository pattern
- **Logging** — log4net

## Project Structure

CollegeAppWebAPI/

├── Configurations/   # AutoMapper profiles and app configuration

├── Controllers/      # API controllers (Login, Student)

├── Data/             # DbContext, entities, repositories

├── Logger/           # Logging setup

├── Migrations/       # EF Core migrations

├── Models/           # DTOs and the APIResponse envelope

├── Validators/       # Custom validation

├── Program.cs        # Application entry point and DI setup

└── appsettings.json  # Configuration (connection string, JWT secret)

## API Response Format

All endpoints return a consistent envelope:

```json
{
  "status": true,
  "statusCode": 200,
  "data": { },
  "errors": []
}
```

- `status` — logical success or failure of the operation
- `statusCode` — the HTTP status represented by the result
- `data` — the payload (object, list, or value)
- `errors` — a list of error messages when `status` is `false`

## Authentication

Obtain a token from the login endpoint, then send it as a Bearer token on protected requests:
Authorization: Bearer <token>

The token contains an `Admin` role claim. Protected student endpoints require this role; `GET /api/Student/Get/{id}` is anonymous.

**Demo credentials:** `Admin` / `Admin123`

## API Endpoints

### Login

| Method | Route        | Description                  | Auth      |
| ------ | ------------ | ---------------------------- | --------- |
| POST   | `/api/Login` | Authenticate and get a token | Anonymous |

### Student

| Method | Route                                  | Description              | Auth      |
| ------ | -------------------------------------- | ------------------------ | --------- |
| GET    | `/api/Student/All`                     | Get all students         | Admin     |
| GET    | `/api/Student/Get/{id}`                | Get a student by id      | Anonymous |
| POST   | `/api/Student/createstudent`           | Create a student         | Admin     |
| PUT    | `/api/Student/updaterecord`            | Update a student record  | Admin     |
| PATCH  | `/api/Student/updaterecordpartial/{id}`| Partially update a student | Admin   |
| DELETE | `/api/Student/deletestudent/{id}`      | Delete a student by id   | Admin     |
| GET    | `/api/Student/LogMessage`              | Logging test endpoint    | Admin     |

## Getting Started

### Prerequisites

- .NET SDK
- SQL Server
- A tool to run requests (Swagger, Postman, or the included `.http` file)

### Configuration

Update `appsettings.json` with your connection string and JWT secret:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CollegeDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JWTSecret": "your-strong-secret-key"
}
```

### Database

Apply the EF Core migrations to create the database:

```bash
dotnet ef database update
```

### Run

```bash
dotnet run
```

The API starts on the URL shown in the console (e.g. `https://localhost:7xxx`). Swagger UI is available at `/swagger` in development.

## CORS

To allow the Angular client to call the API during development, ensure the client origin (`http://localhost:4200`) is permitted and that `UseCors` is registered before `UseAuthentication` and `UseAuthorization` in `Program.cs`.

## Purpose

Built for learning ASP.NET Core Web API, JWT security, Entity Framework Core, the repository pattern, and AutoMapper.