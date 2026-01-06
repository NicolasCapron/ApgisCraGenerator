using Microsoft.EntityFrameworkCore;
using ApgisCraGenerator.Data;
using ApgisCraGenerator.Models;

namespace ApgisCraGenerator.Services;

public class ProjetService
{
    private readonly CraDbContext _context;

    public ProjetService(CraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Projet>> GetProjetsActifsAsync()
    {
        return await _context.Projets
            .Where(p => p.Actif)
            .OrderBy(p => p.CodeActivite)
            .ThenBy(p => p.CodeProjet)
            .ToListAsync();
    }

    public async Task<Projet?> GetProjetByIdAsync(int id)
    {
        return await _context.Projets.FindAsync(id);
    }

    public async Task<Projet> CreateProjetAsync(Projet projet)
    {
        projet.DateCreation = DateTime.Now;
        projet.Actif = true;
        _context.Projets.Add(projet);
        await _context.SaveChangesAsync();
        return projet;
    }

    public async Task UpdateProjetAsync(Projet projet)
    {
        _context.Projets.Update(projet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProjetAsync(int id)
    {
        var projet = await _context.Projets.FindAsync(id);
        if (projet != null)
        {
            _context.Projets.Remove(projet);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ArchiverProjetAsync(int id)
    {
        var projet = await _context.Projets.FindAsync(id);
        if (projet != null)
        {
            projet.Actif = false;
            await _context.SaveChangesAsync();
        }
    }
}