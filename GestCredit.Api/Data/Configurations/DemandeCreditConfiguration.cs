using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestCredit.Api.Data.Configurations;

public class DemandeCreditConfiguration : IEntityTypeConfiguration<DemandeCredit>
{
    public void Configure(EntityTypeBuilder<DemandeCredit> builder)
    {
        builder.ToTable("DemandeCredit");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Montant)
            .HasColumnType("decimal(18,2)") // jamais float/double pour un montant
            .IsRequired();

        builder.Property(d => d.TauxAnnuel)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        // L'enum StatutDemande est stocké comme texte, pas comme entier :
        // ça garde la base lisible et compatible avec la contrainte CHECK du Jour 12.
        builder.Property(d => d.Statut)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.DateCreation)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Mêmes index et mêmes justifications qu'au Jour 14 (SQL) :
        // ClientId est la FK la plus jointe, Statut la colonne la plus filtrée
        // dans les rapports et le workflow de changement de statut.
        builder.HasIndex(d => d.ClientId);
        builder.HasIndex(d => d.Statut);

        // CHECK constraints : reproduisent exactement celles du script 01_schema.sql
        // de la Semaine 3. EF Core ne les génère jamais automatiquement à partir des
        // types C# ou des attributs ; elles doivent être déclarées explicitement ici,
        // sinon la base GestCreditEf accepterait des données que GestCredit refuserait.
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DemandeCredit_Montant", "[Montant] > 0"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DemandeCredit_Duree", "[DureeMois] BETWEEN 1 AND 360"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DemandeCredit_Statut",
            "[Statut] IN ('Brouillon', 'Soumise', 'EnAnalyse', 'Approuvee', 'Rejetee')"));
    }
}