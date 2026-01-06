using System.Text;
using System.Windows;
using ApgisCraGenerator.Data;
using Microsoft.EntityFrameworkCore;

namespace ApgisCraGenerator.Services;

public class CraExportService
{
    private readonly CraDbContext _context;

    public CraExportService(CraDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenererCraAsync(int mois, int annee)
    {
        var taches = await _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.ActiviteSimple)
            .Where(t => t.Mois == mois && t.Annee == annee)
            .ToListAsync();

        var groupes = taches
            .GroupBy(t => t.ProjetOuActivite)
            .Select(g => new
            {
                Code = g.Key,
                Pourcentage = g.Sum(t => t.Pourcentage)
            })
            .OrderByDescending(x => x.Pourcentage)
            .ToList();

        var sb = new StringBuilder();
        for (int i = 0; i < groupes.Count; i++)
        {
            var groupe = groupes[i];
            sb.Append($"{Math.Round(groupe.Pourcentage)}%{groupe.Code}");
            if (i < groupes.Count - 1)
                sb.Append(", ");
        }

        return sb.ToString();
    }

    public void CopierDansPressePapier(string texte)
    {
        Clipboard.SetText(texte);
    }

    public async Task ExporterVersFichierAsync(string texte, int mois, int annee)
    {
        var nomMois = new DateTime(annee, mois, 1).ToString("MMMM", new System.Globalization.CultureInfo("fr-FR"));
        nomMois = char.ToUpper(nomMois[0]) + nomMois.Substring(1);

        var dossier = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CRA"
        );
        Directory.CreateDirectory(dossier);

        var fichier = Path.Combine(dossier, $"Annee {annee}.md");

        var contenu = "";
        if (File.Exists(fichier))
        {
            contenu = await File.ReadAllTextAsync(fichier);
        }

        var ligne = $"{nomMois} : {texte}";

        var lignes = contenu.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        var index = lignes.FindIndex(l => l.StartsWith(nomMois));

        if (index >= 0)
        {
            lignes[index] = ligne;
        }
        else
        {
            lignes.Insert(0, ligne);
        }

        await File.WriteAllTextAsync(fichier, string.Join(Environment.NewLine, lignes));
    }
}