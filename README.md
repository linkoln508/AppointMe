# AppointMe

AppointMe is a booking web API that allows customers to find local businesses
(barbershops, hair salons, massage studios, etc.), select services, and book appointments.

## Database

The project uses PostgreSQL 17 in Docker. The schema is defined by EF Core migrations
(`src/AppointMe.Api/Migrations`).

1. Install Docker Desktop.
2. Clone the repository.
3. Copy `.env.example` to `.env` and set your own password.
4. Start the database (from the repository root): `docker compose up -d`
5. Install the EF Core tool (once): `dotnet tool install --global dotnet-ef`
6. Apply migrations (from `src/AppointMe.Api`): `dotnet ef database update`

To connect with a DB tool (DBeaver, pgAdmin):
- Host: `localhost`
- Port: `5432`
- Database, user and password: from your `.env` file.

To stop the database: `docker compose down`

To add a new migration after changing models or configurations:
`dotnet ef migrations add MigrationName`

Old SQL scripts are kept in `docs/legacy` for history.

## How to run locally

1. Install the .NET SDK (version 10).
2. Set the connection string in User Secrets (from `src/AppointMe.Api`), using the password from your `.env`:
   `dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=appointme;Username=appointme;Password=YOUR_PASSWORD"`
3. Run `dotnet run` in the `src/AppointMe.Api` folder.
4. Open Swagger: `https://localhost:7057/swagger`

## Password policy

- At least 8 characters
- At least one digit (0–9)
- Uppercase, lowercase and special characters are not required
- Email must be unique

Settings are in `Program.cs` (`AddIdentityCore`).

## Authentication

- `POST /api/auth/register` — create account (`Customer` or `BusinessOwner`)
- `POST /api/auth/login` — returns JWT (60 minutes)
- `GET /api/auth/me` — current user from the token (requires `Authorization: Bearer <token>`)

Wrong email or password → `401` with the same message (we do not reveal whether the email exists).
Blocked user → `403`: the password is correct, but access is forbidden.

JWT key is stored in User Secrets:
`dotnet user-secrets set "Jwt:Key" "<at least 32 characters>" --project src/AppointMe.Api`

## Roles and admin

Roles: `Customer`, `BusinessOwner`, `Admin`. Their names are constants in `Auth/Roles.cs`; the roles themselves are created by the `SeedRoles` migration. Registration accepts only `Customer` or `BusinessOwner`, nobody can register as `Admin`.

The admin user is created on startup by `AdminSeeder`, if it does not exist yet:

- email: `Seed:AdminEmail` in `appsettings.json`
- password: `Seed:AdminPassword` in user-secrets (not in git). Minimum 8 characters, at least one digit:

```
dotnet user-secrets set "Seed:AdminPassword" "<your password>" --project src/AppointMe.Api
```

Note: a role change takes effect only after the user gets a new token (the role is stored inside the JWT).