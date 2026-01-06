using Microsoft.EntityFrameworkCore;
using ApgisCraGenerator.Data;
using ApgisCraGenerator.Models;

namespace ApgisCraGenerator.Services;

public class TacheService
{
    private readonly CraDbContext _context;

    public TacheService(CraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tache>> GetTachesAsync(int? mois = null, int? annee = null, StatutTache? statut = null, int? projetId = null)
    {
        var query = _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.ActiviteSimple)
            .AsQueryable();

        if (mois.HasValue)
            query = query.Where(t => t.Mois == mois.Value);

        if (annee.HasValue)
            query = query.Where(t => t.Annee == annee.Value);

        if (statut.HasValue)
            query = query.Where(t => t.Statut == statut.Value);

        if (projetId.HasValue)
            query = query.Where(t => t.ProjetId == projetId.Value);

        return await query.OrderByDescending(t => t.DateCreation).ToListAsync();
    }

    public async Task<Tache?> GetTacheByIdAsync(int id)
    {
        return await _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.ActiviteSimple)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tache> CreateTacheAsync(Tache tache)
    {
        tache.DateCreation = DateTime.Now;
        _context.Taches.Add(tache);
        await _context.SaveChangesAsync();
        return tache;
    }

    public async Task UpdateTacheAsync(Tache tache)
    {
        _context.Taches.Update(tache);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTacheAsync(int id)
    {
        var tache = await _context.Taches.FindAsync(id);
        if (tache != null)
        {
            _context.Taches.Remove(tache);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalPourcentageAsync(int mois, int annee)
    {
        return await _context.Taches
            .Where(t => t.Mois == mois && t.Annee == annee)
            .SumAsync(t => t.Pourcentage);
    }
}