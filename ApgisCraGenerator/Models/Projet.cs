namespace ApgisCraGenerator.Models;

public class Projet
{
    public int Id { get; set; }
    public string CodeActivite { get; set; } = string.Empty;
    public string CodeProjet { get; set; } = string.Empty;
    public string? NumeroLot { get; set; }
    public string? Description { get; set; }
    public DateTime DateCreation { get; set; }
    public bool Actif { get; set; }

    public string FormatComplet => string.IsNullOrEmpty(NumeroLot) 
        ? $"{CodeActivite}-{CodeProjet}" 
        : $"{CodeActivite}-{CodeProjet}-L{NumeroLot}";

    public ICollection<Tache> Taches { get; set; } = new List<Tache>();
}