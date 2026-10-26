# dotnet-rest-api-sample

A small REST API built with **ASP.NET Core 8 (minimal APIs)**, **Entity Framework Core** and **SQLite**, with integration tests, a Dockerfile and a GitHub Actions pipeline.

It is a compact reference for how I structure a .NET backend: endpoints in their own class, validation with proper HTTP status codes, a `DbContext` injected through DI, and tests that run the real HTTP pipeline against a throw-away database.

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/health` | Health check |
| GET | `/api/todos` | List all todos |
| GET | `/api/todos/{id}` | Get one todo (404 if missing) |
| POST | `/api/todos` | Create a todo (400 if the title is blank) |
| PUT | `/api/todos/{id}` | Update title and status |
| DELETE | `/api/todos/{id}` | Delete a todo (204) |

Request body for POST and PUT:

```json
{ "title": "Write README", "isDone": false }
```

## Run

```bash
dotnet run --project src/TaskApi
```

or with Docker:

```bash
docker build -t task-api .
docker run -p 8080:8080 -v task-data:/data task-api
```

## Test

```bash
dotnet test tests/TaskApi.Tests
```

The tests use `WebApplicationFactory` and a separate SQLite file per test, so they exercise routing, validation, serialization and the database together. The same command runs in CI on every push.

## Stack

C# 12, .NET 8, ASP.NET Core minimal APIs, EF Core 8 (SQLite), xUnit, Docker, GitHub Actions.
