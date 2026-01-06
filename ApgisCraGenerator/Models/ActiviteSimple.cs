namespace ApgisCraGenerator.Models;

public class ActiviteSimple
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string Couleur { get; set; } = "#808080";

    public ICollection<Tache> Taches { get; set; } = new List<Tache>();
}