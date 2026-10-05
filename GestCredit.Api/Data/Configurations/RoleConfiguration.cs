using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestCredit.Api.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nom)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Nom).IsUnique();

        // Mêmes 3 rôles, mêmes Id, que le seed SQL du Jour 12 (02_seed.sql) :
        // Agent=1, Analyste=2, Administrateur=3.
        builder.HasData(
            new Role { Id = 1, Nom = "Agent" },
            new Role { Id = 2, Nom = "Analyste" },
            new Role { Id = 3, Nom = "Administrateur" }
        );
    }
}
