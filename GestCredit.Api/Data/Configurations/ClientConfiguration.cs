using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestCredit.Api.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(150); // aligné sur NVARCHAR(150) du Jour 12

        builder.Property(c => c.Ville)
            .IsRequired()
            .HasMaxLength(100); // aligné sur NVARCHAR(100) du Jour 12

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.HasIndex(c => c.Email).IsUnique(); // reflète UQ_Client_Email du Jour 12

        builder.HasMany(c => c.Demandes)
            .WithOne(d => d.Client)
            .HasForeignKey(d => d.ClientId);
    }
}