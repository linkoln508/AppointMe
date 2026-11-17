using AppointMe.Api.Auth;
using AppointMe.Api.Data;
using AppointMe.Api.Middleware;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    // принимаем Bearer-токен
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Вставь accessToken из /api/auth/login (без слова Bearer)"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    });
});

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

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
// настройки JWT
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

// проверка JWT в каждом запросе
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                 // кто выдал
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,               // для кого
            ValidAudience = jwt.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            NameClaimType = "name",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

builder.Services.AddScoped<IBusinessRepository, EfBusinessRepository>();

builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

app.UseMiddleware<RequestTimingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "AppointMe v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
