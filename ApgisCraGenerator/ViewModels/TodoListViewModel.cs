using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ApgisCraGenerator.Models;
using ApgisCraGenerator.Services;

namespace ApgisCraGenerator.ViewModels;

public partial class TodoListViewModel : ObservableObject
{
    private readonly TacheService _tacheService;
    private readonly ProjetService _projetService;

    [ObservableProperty]
    private ObservableCollection<Tache> _taches = new();

    [ObservableProperty]
    private ObservableCollection<Projet> _projetsDisponibles = new();

    [ObservableProperty]
    private Tache? _selectedTache;

    [ObservableProperty]
    private string _titre = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int? _projetId;

    [ObservableProperty]
    private decimal _pourcentage;

    [ObservableProperty]
    private DateTime? _dateDebut;

    [ObservableProperty]
    private DateTime? _dateFin;

    [ObservableProperty]
    private StatutTache _statut = StatutTache.AFaire;

    [ObservableProperty]
    private int _moisFiltre = DateTime.Now.Month;

    [ObservableProperty]
    private int _anneeFiltre = DateTime.Now.Year;

    [ObservableProperty]
    private StatutTache? _statutFiltre;

    [ObservableProperty]
    private bool _isEditing;

    private int? _editingTacheId;

    public ObservableCollection<StatutTache> StatutsDisponibles { get; } = new()
    {
        StatutTache.AFaire,
        StatutTache.EnCours,
        StatutTache.Terminee
    };

    public TodoListViewModel(TacheService tacheService, ProjetService projetService)
    {
        _tacheService = tacheService;
        _projetService = projetService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadProjetsAsync();
        await LoadTachesAsync();
    }

    [RelayCommand]
    private async Task LoadProjetsAsync()
    {
        var projets = await _projetService.GetProjetsActifsAsync();
        ProjetsDisponibles.Clear();
        foreach (var projet in projets)
        {
            ProjetsDisponibles.Add(projet);
        }
    }

    [RelayCommand]
    private async Task LoadTachesAsync()
    {
        var taches = await _tacheService.GetTachesAsync(MoisFiltre, AnneeFiltre, StatutFiltre, null);
        Taches.Clear();
        foreach (var tache in taches)
        {
            Taches.Add(tache);
        }
    }

    [RelayCommand]
    private void NouvelleTache()
    {
        Titre = string.Empty;
        Description = string.Empty;
        ProjetId = null;
        Pourcentage = 0;
        DateDebut = null;
        DateFin = null;
        Statut = StatutTache.AFaire;
        IsEditing = false;
        _editingTacheId = null;
    }

    [RelayCommand]
    private void ModifierTache()
    {
        if (SelectedTache == null) return;

        Titre = SelectedTache.Titre;
        Description = SelectedTache.Description ?? string.Empty;
        ProjetId = SelectedTache.ProjetId;
        Pourcentage = SelectedTache.Pourcentage;
        DateDebut = SelectedTache.DateDebut;
        DateFin = SelectedTache.DateFin;
        Statut = SelectedTache.Statut;
        IsEditing = true;
        _editingTacheId = SelectedTache.Id;
    }

    [RelayCommand]
    private async Task EnregistrerTacheAsync()
    {
        if (string.IsNullOrWhiteSpace(Titre))
        {
            MessageBox.Show("Le titre est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditing && _editingTacheId.HasValue)
        {
            var tache = await _tacheService.GetTacheByIdAsync(_editingTacheId.Value);
            if (tache != null)
            {
                tache.Titre = Titre;
                tache.Description = string.IsNullOrWhiteSpace(Description) ? null : Description;
                tache.ProjetId = ProjetId;
                tache.Pourcentage = Pourcentage;
                tache.DateDebut = DateDebut;
                tache.DateFin = DateFin;
                tache.Statut = Statut;
                tache.Mois = MoisFiltre;
                tache.Annee = AnneeFiltre;
                
                if (Statut == StatutTache.Terminee && tache.DateRealisation == null)
                {
                    tache.DateRealisation = DateTime.Now;
                }
                
                await _tacheService.UpdateTacheAsync(tache);
            }
        }
        else
        {
            var nouvelleTache = new Tache
            {
                Titre = Titre,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                ProjetId = ProjetId,
                Pourcentage = Pourcentage,
                DateDebut = DateDebut,
                DateFin = DateFin,
                Statut = Statut,
                Mois = MoisFiltre,
                Annee = AnneeFiltre,
                DateRealisation = Statut == StatutTache.Terminee ? DateTime.Now : null
            };
            await _tacheService.CreateTacheAsync(nouvelleTache);
        }

        await LoadTachesAsync();
        NouvelleTache();
    }

    [RelayCommand]
    private async Task SupprimerTacheAsync()
    {
        if (SelectedTache == null) return;

        var result = MessageBox.Show(
            $"Voulez-vous vraiment supprimer la tâche '{SelectedTache.Titre}' ?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _tacheService.DeleteTacheAsync(SelectedTache.Id);
            await LoadTachesAsync();
        }
    }

    [RelayCommand]
    private async Task MarquerTermineeAsync()
    {
        if (SelectedTache == null) return;

        SelectedTache.Statut = StatutTache.Terminee;
        SelectedTache.DateRealisation = DateTime.Now;
        await _tacheService.UpdateTacheAsync(SelectedTache);
        await LoadTachesAsync();
    }

    partial void OnMoisFiltreChanged(int value) => _ = LoadTachesAsync();
    partial void OnAnneeFiltreChanged(int value) => _ = LoadTachesAsync();
    partial void OnStatutFiltreChanged(StatutTache? value) => _ = LoadTachesAsync();
}