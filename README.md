# AppointMe

AppointMe is a small booking app. Customers can find local businesses (barbers, hair salons, massage and so on), look at their services and book a time. The backend is an ASP.NET Core Web API, the frontend is a simple site on jQuery.

## Database

We use PostgreSQL 17 running in Docker. Tables are created by EF Core migrations (they are in `src/AppointMe.Api/Migrations`).

First time setup:

1. Install Docker Desktop.
2. Clone the repo.
3. Copy `.env.example` to `.env` and put your own password there.
4. Start the database from the repo root: `docker compose up -d`
5. Install the EF Core tool (only once): `dotnet tool install --global dotnet-ef`
6. Create the tables (from `src/AppointMe.Api`): `dotnet ef database update`

To look at the data in DBeaver or pgAdmin use host `localhost`, port `5432`, and the database name, user and password from your `.env`.

Stop the database: `docker compose down`

If you change a model or a configuration, add a new migration:
`dotnet ef migrations add MigrationName`

The old SQL scripts are still in `docs/legacy`, just for history.

## Running the API

1. You need the .NET 10 SDK.
2. Put the connection string into User Secrets (run it from `src/AppointMe.Api`, use the password from `.env`):
   `dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=appointme;Username=appointme;Password=YOUR_PASSWORD"`
3. Run `dotnet run` in `src/AppointMe.Api`.
4. Swagger is at `https://localhost:7057/swagger`.

## Passwords

A password needs at least 8 characters and at least one digit. Capital letters and special characters are not required. One email can be registered only once.

These rules are set in `Program.cs` (in `AddIdentityCore`).

## Login and tokens

- `POST /api/auth/register` creates an account. The account type is `Customer` or `BusinessOwner`.
- `POST /api/auth/login` gives you a JWT token. It works for 60 minutes.
- `GET /api/auth/me` shows who you are (send the token in the `Authorization: Bearer <token>` header).

If the email or the password is wrong, you get `401` and always the same message, so nobody can find out which emails are registered. If the user is blocked, you get `403`: the password is right, but the user is not allowed in.

The secret key for signing tokens is in User Secrets:
`dotnet user-secrets set "Jwt:Key" "<at least 32 characters>" --project src/AppointMe.Api`

## Roles and the admin

There are three roles: `Customer`, `BusinessOwner` and `Admin`. Their names are in `Auth/Roles.cs`, and the roles are added to the database by the `SeedRoles` migration. You can register only as `Customer` or `BusinessOwner`, nobody can register as `Admin`.

The admin account is created by `AdminSeeder` when the app starts (only if it doesn't exist yet). The email comes from `Seed:AdminEmail` in `appsettings.json`, and the password from User Secrets, so it never gets into git (at least 8 characters and one digit):

```
dotnet user-secrets set "Seed:AdminPassword" "<your password>" --project src/AppointMe.Api
```

The role is saved inside the token. So if you change someone's role, they will see it only after they log in again.

### Who can do what

| Endpoint | Who can call it |
|---|---|
| `POST /api/auth/register`, `POST /api/auth/login` | Anyone |
| `GET /api/auth/me` | Any logged in user |
| `GET /api/ping` | Anyone |
| `GET /api/categories`, `GET /api/categories/{id}` | Anyone |
| `POST/PUT/DELETE /api/categories` | Only `Admin` |
| `GET /api/businesses`, `GET /api/businesses/{id}` | Anyone |
| `GET /api/businesses/{id}/services`, `GET .../services/{serviceId}` | Anyone |
| `POST /api/businesses` | `BusinessOwner` (the owner is taken from the token) |
| `PUT/DELETE /api/businesses/{id}` | The owner of this business or `Admin` |
| `POST/PUT/DELETE /api/businesses/{id}/services` | The owner of this business or `Admin` |
| `GET /api/me/businesses` | `BusinessOwner` (only their own businesses) |

Without a token the API returns `401`. With a token but the wrong role it returns `403`.
By default every endpoint needs a login (this is the fallback policy). Public ones have `[AllowAnonymous]`.
When an owner creates a business, `ownerId` is taken from the token, not from the request body, so you can't create a business for someone else.

### Checking the role vs checking the owner

Checking the role (`[Authorize(Roles = ...)]`) only answers "what kind of user is this?". It looks at the role in the token and doesn't know anything about the business in the request.

Checking the owner answers "is this business yours?". For that we need the business itself, so the controller first loads it from the database and then calls `IAuthorizationService.AuthorizeAsync(User, business, "BusinessOwnerOfResource")`. `BusinessOwnerHandler` compares the `OwnerId` of the business with the user id from the token. Admin is allowed for any business.

We need both: the role check lets only owners and admins into the method, and the owner check decides if this exact business can be changed. If the business doesn't exist you get `404`, if it's not yours you get `403`.

## Frontend

The site is just HTML, CSS and jQuery, there is no build step. It's in `src/web` and it talks to the API with Ajax requests from the browser.

Install the small web server once:

```
dotnet tool install --global dotnet-serve
```

To run it:

1. Start Docker Desktop (the database must be running).
2. Start the API.
3. From the repo root run:

```
dotnet serve --directory src/web --port 5500
```

4. Open `http://localhost:5500`.

The API address is in `src/web/js/config.js` (`API_URL`). If your API runs on another port, change it there.

Tip: keep "Disable cache" on in DevTools (F12, Network tab). Otherwise the browser can keep old JS files and your changes won't show up.

### CORS

The site (port 5500) and the API (port 7057) are different origins for the browser, and by default the browser doesn't let one origin read answers from another. So the API allows `http://localhost:5500` with the `Frontend` CORS policy in `Program.cs`. That's why the site has to run on port 5500.

### Login on the frontend

After login we save the token, the time when it expires and the user info in `localStorage`. The logic for this is in `js/auth.js`:

- it adds the token to every request to the API;
- if the API answers `401`, it logs the user out and sends them to the login page;
- it builds the menu in the header for the user's role;
- `requireRole` closes pages for the wrong roles (a guest goes to the login page and comes back after login, a user with the wrong role gets the Forbidden page).

These page checks are just for convenience. The real protection is in the API.

**The risk of `localStorage`:** any JavaScript on our pages can read it. If someone manages to put their own script on our site (this is called XSS), the script can steal the token and use the API as that user until the token expires. To make this harder, we never put data from the API into the page as raw HTML, only as text (`escapeHtml` in `js/utils.js` and `.text()`). Tokens also live only 60 minutes. A safer option would be an `httpOnly` cookie (JavaScript can't read it), but then we'd need CSRF protection and a different setup on the API side, so for this project we keep `localStorage`.
