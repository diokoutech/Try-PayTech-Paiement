using Microsoft.EntityFrameworkCore;
using TestPayTech.Database.Entities;

namespace TestPayTech.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paiement> Paiements { get; set; }
}