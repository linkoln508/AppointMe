using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointMe.Api.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services", t =>
        {
            t.HasCheckConstraint("ck_services_price_positive", "price > 0");
            t.HasCheckConstraint("ck_services_duration_range", "duration_minutes BETWEEN 5 AND 480");
        });

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

        builder.Property(s => s.Price)
            .HasPrecision(10, 2);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);
    }
}