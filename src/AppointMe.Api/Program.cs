using AppointMe.Api.Data;
using AppointMe.Api.Middleware;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Подключение к базе (лежит в User Secrets)
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppointMeDbContext>(options =>
    options.UseNpgsql(connectionString)          // PostgreSQL, подключения из User Secrets
           .UseSnakeCaseNamingConvention());

// Identity: пользователи и роли
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole<Guid>>()                       // включить роли
    .AddEntityFrameworkStores<AppointMeDbContext>();      // хранить юзеров/роли через DbContext

builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

builder.Services.AddScoped<IBusinessRepository, EfBusinessRepository>();

builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "AppointMe v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
