using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointMe.Api.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);                   // Id (PK)

        builder.Property(c => c.Name)
            .IsRequired()                            // NOT NULL
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Slug).IsUnique();

        builder.Property(c => c.Id)
            .UseIdentityByDefaultColumn()
            .HasIdentityOptions(startValue: 13);

        builder.HasData(
            new Category { Id = 1, Name = "Barber", Slug = "barber" },
            new Category { Id = 2, Name = "Hair Salon", Slug = "hair-salon" },
            new Category { Id = 3, Name = "Nail Salon", Slug = "nail-salon" },
            new Category { Id = 4, Name = "Physical Therapy", Slug = "physical-therapy" },
            new Category { Id = 5, Name = "Dentist", Slug = "dentist" },
            new Category { Id = 6, Name = "Car Detailing", Slug = "car-detailing" },
            new Category { Id = 7, Name = "Tire Service", Slug = "tire-service" },
            new Category { Id = 8, Name = "Tattoo Studio", Slug = "tattoo-studio" },
            new Category { Id = 9, Name = "Personal Trainer", Slug = "personal-trainer" },
            new Category { Id = 10, Name = "Massage Therapy", Slug = "massage-therapy" },
            new Category { Id = 11, Name = "Yoga Studio", Slug = "yoga-studio" },
            new Category { Id = 12, Name = "Veterinary Clinic", Slug = "veterinary-clinic" }
        );
    }
}