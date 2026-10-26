# dotnet-rest-api-sample

A small REST API built with **ASP.NET Core 8 (minimal APIs)** and **Entity Framework Core** on **SQL Server** (or SQLite for quick local runs), with integration tests, Docker Compose, Azure infrastructure as code (Bicep) and a GitHub Actions pipeline.

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

or with Docker (SQLite file in a volume):

```bash
docker build -t task-api .
docker run -p 8080:8080 -v task-data:/data task-api
```

### With SQL Server

The provider is chosen from the connection string: a `Server=...` string uses SQL Server, anything else uses SQLite. `docker-compose.yml` starts the API together with SQL Server 2022:

```bash
SA_PASSWORD='choose-a-strong-Passw0rd!' docker compose up --build
```

The database is created on first start (`EnsureCreated`; a real project would use EF migrations).

## Azure

`infra/main.bicep` defines the cloud setup: an Azure SQL server and database, a Linux App Service plan and a web app with the SQL connection string as an app setting and `/health` as the health check. The `Deploy to Azure` workflow provisions it with `az deployment group create` and publishes the app with `azure/webapps-deploy`, authenticating through OIDC (secrets and variables are listed at the top of the workflow file).

The workflow is manual-only. CI checks that the Bicep file compiles, but the template has not been deployed to a live subscription. For production I would replace the SQL password with a managed identity and Key Vault references.

## Test

```bash
dotnet test tests/TaskApi.Tests
```

The tests use `WebApplicationFactory` and a separate SQLite file per test, so they exercise routing, validation, serialization and the database together. To run the same tests against SQL Server, set `TASKAPI_TEST_CONNECTION` to a SQL Server connection string. CI runs both variants (SQLite and a SQL Server 2022 service container) on every push.

## Stack

C# 12, .NET 8, ASP.NET Core minimal APIs, EF Core 8 (SQLite and SQL Server), xUnit, Docker / Docker Compose, Bicep, GitHub Actions.
