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
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>(); // Jour 20
    public DbSet<Role> Roles => Set<Role>();                      // Jour 20

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestCreditDbContext).Assembly);
    }
}
