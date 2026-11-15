using Inventory.Core.Constants;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Description).HasMaxLength(200);
        builder.HasIndex(r => r.Name).IsUnique();

        var adminRole = new Role()
        {
            Id = 1,
            Name = RoleNames.Admin
        };

        var viewerRole = new Role()
        {
            Id = 2,
            Name = RoleNames.Viewer
        };

        builder.HasData(adminRole, viewerRole);
    }
}
