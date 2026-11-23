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

### Who can call what

| Endpoint | Who can call it |
|---|---|
| `POST /api/auth/register`, `POST /api/auth/login` | Anyone (anonymous) |
| `GET /api/auth/me` | Any authenticated user |
| `GET /api/ping` | Anyone (anonymous) |
| `GET /api/categories`, `GET /api/categories/{id}` | Anyone (anonymous) |
| `POST/PUT/DELETE /api/categories` | `Admin` |
| `GET /api/businesses`, `GET /api/businesses/{id}` | Anyone (anonymous) |
| `GET /api/businesses/{id}/services`, `GET .../services/{serviceId}` | Anyone (anonymous) |
| `POST /api/businesses` | `BusinessOwner` (owner is taken from the token) |
| `PUT/DELETE /api/businesses/{id}` | Owner of this business, or `Admin` |
| `POST/PUT/DELETE /api/businesses/{id}/services` | Owner of this business, or `Admin` |
| `GET /api/me/businesses` | `BusinessOwner` (returns only own businesses) |

No token → `401 Unauthorized`. Valid token but wrong role → `403 Forbidden`.
Everything is protected by default (fallback policy); public endpoints are marked `[AllowAnonymous]`.
`ownerId` of a new business is taken from the token, not from the request body.

### Role-based vs resource-based authorization

**Role-based** authorization answers "what kind of user are you?". `[Authorize(Roles = ...)]`
checks the role from the token. It does not know which object the request is about.

**Resource-based** authorization answers "is this specific object yours?". It needs the
object itself, so the controller first loads the business from the database and then calls
`IAuthorizationService.AuthorizeAsync(User, business, "BusinessOwnerOfResource")`.
The handler (`BusinessOwnerHandler`) compares `OwnerId` of the business with the user id
from the token. `Admin` passes for any business.

Both are needed: the role attribute lets only owners and admins into the method,
and the policy decides whether this particular business may be changed.
Not found → `404`, found but not yours → `403`.

## Frontend

A static site built with HTML, CSS and jQuery (Ajax). It lives in `src/web` and calls the API from the browser.

### One-time setup
```
dotnet tool install --global dotnet-serve
```

### How to run

1. Start Docker Desktop with the postgres container.
2. Start the API (`AppointMe.Api`).
3. From the repository root, start the site:
```
dotnet serve --directory src/web --port 5500
```

4. Open `http://localhost:5500`.

### API address

It is set in `src/web/js/config.js` (the `API_URL` constant). If the API runs on a different port, change it there.

### CORS

A browser does not let a page from one origin read responses from another origin, so the API explicitly allows requests from `http://localhost:5500` (the `Frontend` policy in `Program.cs`). The site must be opened on port 5500, otherwise the requests will be blocked.