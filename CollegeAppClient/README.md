# CollegeAppClient

Frontend for CollegeApp — an Angular single-page application for managing student records. It authenticates against the CollegeAppWebAPI, stores the JWT, and provides a UI for login and full CRUD operations.

## Tech Stack

- **Framework** — Angular (standalone components)
- **Language** — TypeScript
- **HTTP** — Angular HttpClient with a functional interceptor
- **Routing** — Angular Router with route guards

## Features

- Login page with JWT authentication
- View all students
- Add a new student
- Edit a student inline
- Delete a student
- Automatic Bearer-token attachment via an HTTP interceptor
- Route protection via an auth guard

## Project Structure

```
src/app/
├── components/
│   ├── login/                  # Login page
│   └── student-list/           # Student list with create, edit, delete
├── services/
│   ├── auth.service.ts         # Login, token storage, session state
│   └── student.service.ts      # Student CRUD calls
├── interceptors/
│   └── auth.interceptor.ts     # Attaches the Bearer token
├── guards/
│   └── auth.guard.ts           # Protects authenticated routes
├── models/                     # Student, login, and APIResponse interfaces
├── app.config.ts               # App providers (router, http, interceptor)
└── app.routes.ts               # Route definitions
```

## API Integration

The client consumes the CollegeAppWebAPI. All responses share a common envelope, and the data payload is read from the `data` property:

```json
{
  "status": true,
  "statusCode": 200,
  "data": { },
  "errors": []
}
```

Note that the API returns business failures (such as invalid credentials) with an HTTP 200 status and `status: false`, so the client checks the `status` field rather than relying solely on HTTP error codes.

## Getting Started

### Prerequisites

- Node.js and npm
- Angular CLI
- A running instance of CollegeAppWebAPI

### Install

```bash
npm install
```

### Configure the API URL

Set the backend base URL in `src/environments/environment.ts`:

```ts
export const environment = {
  apiUrl: 'https://localhost:7xxx/api'  // match your API port
};
```

### Run

```bash
ng serve
```

Open `http://localhost:4200`.

**Demo credentials:** `Admin` / `Admin123`

## Notes

- Ensure the API has CORS configured to allow `http://localhost:4200`.
- The JWT is stored in the browser and attached to every request automatically by the interceptor.

## Purpose

Built for learning Angular standalone components, JWT-based authentication, HTTP interceptors, route guards, and integration with an ASP.NET Core Web API.
