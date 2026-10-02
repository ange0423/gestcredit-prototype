using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestCredit.Api.Data;

public class GestCreditDbContext : DbContext
{
    public GestCreditDbContext(DbContextOptions<GestCreditDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<DemandeCredit> Demandes => Set<DemandeCredit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Charge automatiquement toutes les classes IEntityTypeConfiguration<T>
        // du projet, plutôt que de tout écrire ici en dur.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestCreditDbContext).Assembly);
    }
}