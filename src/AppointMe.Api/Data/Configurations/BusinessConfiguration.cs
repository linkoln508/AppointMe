using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointMe.Api.Data.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("businesses");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.City).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Address).IsRequired().HasMaxLength(300);
        builder.Property(b => b.Phone).HasMaxLength(30);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("now()");

        builder.Ignore(b => b.CategoryName);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Services)
            .WithOne()
            .HasForeignKey(s => s.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}