using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppointMe.Api.Auth;

namespace AppointMe.Api.Data.Configurations;

// конфигурация для таблицы roles (класс ролей из Identity)
public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("8d04dce2-969a-435d-bba4-df3f325983dc"),
                Name = Roles.Customer,
                NormalizedName = Roles.Customer.ToUpperInvariant(),
                ConcurrencyStamp = "8d04dce2-969a-435d-bba4-df3f325983dc"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("b6d1b5f0-3c4e-4a8f-9d2e-6f7a1c2b3d4e"),
                Name = Roles.BusinessOwner,
                NormalizedName = Roles.BusinessOwner.ToUpperInvariant(),
                ConcurrencyStamp = "b6d1b5f0-3c4e-4a8f-9d2e-6f7a1c2b3d4e"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("e2a7c9d4-5b1f-4e3a-8c6d-0f9b2a1e7c35"),
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = "e2a7c9d4-5b1f-4e3a-8c6d-0f9b2a1e7c35"
            }
        );
    }
}