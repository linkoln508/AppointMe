# AppointMe

AppointMe is a booking web API that allows customers to find local businesses
(barbershops, hair salons, massage studios, etc.), select services, and book appointments.

## Database

The project uses PostgreSQL 17 in Docker.

1. Install Docker Desktop.
2. Clone the repository.
3. Copy `.env.example` to `.env` and set your own password.
4. Start the database: `docker compose up -d`
5. Connect with any DB tool (DBeaver, pgAdmin):
   - Host: `localhost`
   - Port: `5432`
   - Database, user and password: from your `.env` file.

To stop the database: `docker compose down`

## How to run locally

1. Install the .NET SDK (version 10).
2. Run the command `dotnet run` in the `src/AppointMe.Api` folder.

### Create the schema

Run all scripts from `db/scripts` in order (from the repository root).
Use the user and database from your `.env` file:

    Get-Content db\scripts\*.sql | docker exec -i appointme-postgres psql -U appointme -d appointme

Expected output: `CREATE TABLE` three times and `INSERT 0 12`.