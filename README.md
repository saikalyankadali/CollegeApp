# CollegeApp

A full-stack student records management application built for learning and practice. The backend is an ASP.NET Core Web API secured with JWT authentication; the frontend is an Angular single-page application that consumes it.

## Overview

CollegeApp lets an authenticated administrator manage student records — create, read, update, and delete — through a clean web interface backed by a secured REST API.

| Layer    | Technology                                              |
| -------- | ------------------------------------------------------- |
| Backend  | ASP.NET Core Web API, Entity Framework Core, SQL Server |
| Security | JWT Bearer authentication, role-based authorization     |
| Frontend | Angular (standalone components), TypeScript             |
| Database | SQL Server (CollegeDB)                                  |

## Repository Structure

```
CollegeApp/
├── CollegeAppWebAPI/           # ASP.NET Core Web API (backend)
│   ├── README.md
├── CollegeAppClient/           # Angular SPA (frontend)
│   ├── README.md                    
├── Readme.md/                  # You are here
```

## Architecture

```
┌──────────────────┐    HTTPS / JWT     ┌──────────────────┐      EF Core      ┌──────────────┐
│ CollegeAppClient │ ─────────────────> │ CollegeAppWebAPI │ ────────────────> │  SQL Server  │
│  (Angular SPA)   │                    │   (REST API)     │                   │  (CollegeDB) │
│                  │ <───────────────── │                  │ <──────────────── │              │
└──────────────────┘   APIResponse JSON └──────────────────┘    Student data   └──────────────┘
```

The client authenticates against the API's `/api/Login` endpoint, stores the returned JWT,
and attaches it as a `Bearer` token on every subsequent request. The API validates the token,
enforces role-based access, and persists data to SQL Server through Entity Framework Core.

The client authenticates against the API's `/api/Login` endpoint, stores the returned JWT, and attaches it as a `Bearer` token on every subsequent request. The API validates the token and enforces role-based access on protected endpoints.

## Getting Started

Each project has its own setup instructions. Start with the backend, then the frontend.

1. **Backend** — see [`CollegeAppWebAPI/README.md`](./CollegeAppWebAPI/README.md)
2. **Frontend** — see [`CollegeAppClient/README.md`](./CollegeAppClient/README.md)

Default credentials for the demo: **Username** `Admin` · **Password** `Admin123`

## Features

- JWT-based authentication with role-based authorization (`Admin`)
- Full CRUD operations on student records
- Consistent API response envelope across all endpoints
- Repository pattern and AutoMapper on the backend
- Standalone-component Angular frontend with route guards and an HTTP interceptor

## Purpose

This project is built for hands-on learning and practice with ASP.NET Core Web API, JWT security, Entity Framework Core, and Angular integration. It is not intended for production use.

## License

This project is for educational purposes.
