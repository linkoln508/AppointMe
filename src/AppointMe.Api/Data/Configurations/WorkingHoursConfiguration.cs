using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointMe.Api.Data.Configurations;

public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
{
    public void Configure(EntityTypeBuilder<WorkingHours> builder)
    {
        builder.ToTable("working_hours");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(w => new { w.BusinessId, w.DayOfWeek })
            .IsUnique();

        builder.HasOne<Business>()
            .WithMany()
            .HasForeignKey(w => w.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}