using Npgsql;
using AppointMe.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// ===== (DI) =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Подключение в User Secrets
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString!));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

// Dapper: столбец category_id из базы = свойство CategoryId в C#.
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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
