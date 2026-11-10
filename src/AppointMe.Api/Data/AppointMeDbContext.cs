using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointMe.Api.Data;
public class AppointMeDbContext : DbContext
{
    public AppointMeDbContext(DbContextOptions<AppointMeDbContext> options) : base(options)
    {
    }
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Business> Businesses => Set<Business>();

    public DbSet<Service> Services => Set<Service>();

    // EF Core вызывает когда строит модель базы
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointMeDbContext).Assembly);
    }

}