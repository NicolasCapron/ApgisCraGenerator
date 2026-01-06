namespace ApgisCraGenerator.Models;

public class Tache
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProjetId { get; set; }
    public Projet? Projet { get; set; }
    public int? ActiviteSimpleId { get; set; }
    public ActiviteSimple? ActiviteSimple { get; set; }
    public decimal Pourcentage { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public DateTime? DateRealisation { get; set; }
    public StatutTache Statut { get; set; }
    public int Mois { get; set; }
    public int Annee { get; set; }

    public string ProjetOuActivite => Projet?.FormatComplet ?? ActiviteSimple?.Code ?? "N/A";
}