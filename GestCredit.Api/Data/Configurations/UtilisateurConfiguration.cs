using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestCredit.Api.Data.Configurations;

public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> builder)
    {
        builder.ToTable("Utilisateur");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.NomUtilisateur)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.NomUtilisateur).IsUnique();

        builder.Property(u => u.MotDePasseHash)
            .IsRequired()
            .HasMaxLength(300);

        // EF Core génère et gère la table de jonction UtilisateurRole lui-même
        // (clé composite UtilisateurId+RoleId), exactement comme au Jour 12.
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Utilisateurs)
            .UsingEntity(j => j.ToTable("UtilisateurRole"));
    }
}
