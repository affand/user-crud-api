# User CRUD API

Small ASP.NET Core Web API for a coding interview CRUD exercise. It uses an in-memory repository so it can run without a database.

## Projects

- `UserCrudApi.Api` - ASP.NET Core API
- `UserCrudApi.Tests` - xUnit unit tests

## Endpoints

- `GET /api/users` - list users
- `GET /api/users/{id}` - get one user
- `POST /api/users` - create a user
- `PUT /api/users/{id}` - update a user
- `DELETE /api/users/{id}` - delete a user

## Run

```powershell
dotnet run --project .\UserCrudApi.Api\UserCrudApi.Api.csproj
```

## Test

```powershell
dotnet test
```

The tests cover create, duplicate email handling, list, get by id, update, update conflicts, delete, and not-found cases.
