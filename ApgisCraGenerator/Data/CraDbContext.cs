using Microsoft.EntityFrameworkCore;
using ApgisCraGenerator.Models;

namespace ApgisCraGenerator.Data;

public class CraDbContext : DbContext
{
    public DbSet<Projet> Projets { get; set; }
    public DbSet<ActiviteSimple> ActivitesSimples { get; set; }
    public DbSet<Tache> Taches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ApgisCraGenerator",
            "cra.db"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tache>()
            .HasOne(t => t.Projet)
            .WithMany(p => p.Taches)
            .HasForeignKey(t => t.ProjetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Tache>()
            .HasOne(t => t.ActiviteSimple)
            .WithMany(a => a.Taches)
            .HasForeignKey(t => t.ActiviteSimpleId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ActiviteSimple>().HasData(
            new ActiviteSimple { Id = 1, Code = "CONG", Libelle = "Congés", Couleur = "#4CAF50" },
            new ActiviteSimple { Id = 2, Code = "FORT", Libelle = "Formation", Couleur = "#2196F3" },
            new ActiviteSimple { Id = 3, Code = "ABS", Libelle = "Absence", Couleur = "#FF9800" },
            new ActiviteSimple { Id = 4, Code = "MALA", Libelle = "Maladie", Couleur = "#F44336" }
        );
    }
}