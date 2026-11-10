using Npgsql;
using AppointMe.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== (DI) =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Подключение к базе (лежит в User Secrets)
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppointMeDbContext>(options =>
    options.UseNpgsql(connectionString)          // PostgreSQL, подключения из User Secrets
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

builder.Services.AddScoped<IBusinessRepository, EfBusinessRepository>();

builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "AppointMe v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
