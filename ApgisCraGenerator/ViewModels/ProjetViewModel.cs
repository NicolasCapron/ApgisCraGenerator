using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ApgisCraGenerator.Models;
using ApgisCraGenerator.Services;

namespace ApgisCraGenerator.ViewModels;

public partial class ProjetViewModel : ObservableObject
{
    private readonly ProjetService _projetService;

    [ObservableProperty]
    private ObservableCollection<Projet> _projets = new();

    [ObservableProperty]
    private Projet? _selectedProjet;

    [ObservableProperty]
    private string _codeActivite = string.Empty;

    [ObservableProperty]
    private string _codeProjet = string.Empty;

    [ObservableProperty]
    private string _numeroLot = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    private int? _editingProjetId;

    public ProjetViewModel(ProjetService projetService)
    {
        _projetService = projetService;
        _ = LoadProjetsAsync();
    }

    [RelayCommand]
    private async Task LoadProjetsAsync()
    {
        var projets = await _projetService.GetProjetsActifsAsync();
        Projets.Clear();
        foreach (var projet in projets)
        {
            Projets.Add(projet);
        }
    }

    [RelayCommand]
    private void NouveauProjet()
    {
        CodeActivite = string.Empty;
        CodeProjet = string.Empty;
        NumeroLot = string.Empty;
        Description = string.Empty;
        IsEditing = false;
        _editingProjetId = null;
    }

    [RelayCommand]
    private void ModifierProjet()
    {
        if (SelectedProjet == null) return;

        CodeActivite = SelectedProjet.CodeActivite;
        CodeProjet = SelectedProjet.CodeProjet;
        NumeroLot = SelectedProjet.NumeroLot ?? string.Empty;
        Description = SelectedProjet.Description ?? string.Empty;
        IsEditing = true;
        _editingProjetId = SelectedProjet.Id;
    }

    [RelayCommand]
    private async Task EnregistrerProjetAsync()
    {
        if (string.IsNullOrWhiteSpace(CodeActivite) || string.IsNullOrWhiteSpace(CodeProjet))
        {
            MessageBox.Show("Le code activité et le code projet sont obligatoires.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditing && _editingProjetId.HasValue)
        {
            var projet = await _projetService.GetProjetByIdAsync(_editingProjetId.Value);
            if (projet != null)
            {
                projet.CodeActivite = CodeActivite;
                projet.CodeProjet = CodeProjet;
                projet.NumeroLot = string.IsNullOrWhiteSpace(NumeroLot) ? null : NumeroLot;
                projet.Description = string.IsNullOrWhiteSpace(Description) ? null : Description;
                await _projetService.UpdateProjetAsync(projet);
            }
        }
        else
        {
            var nouveauProjet = new Projet
            {
                CodeActivite = CodeActivite,
                CodeProjet = CodeProjet,
                NumeroLot = string.IsNullOrWhiteSpace(NumeroLot) ? null : NumeroLot,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description
            };
            await _projetService.CreateProjetAsync(nouveauProjet);
        }

        await LoadProjetsAsync();
        NouveauProjet();
    }

    [RelayCommand]
    private async Task SupprimerProjetAsync()
    {
        if (SelectedProjet == null) return;

        var result = MessageBox.Show(
            $"Voulez-vous vraiment supprimer le projet {SelectedProjet.FormatComplet} ?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _projetService.DeleteProjetAsync(SelectedProjet.Id);
            await LoadProjetsAsync();
        }
    }

    [RelayCommand]
    private async Task ArchiverProjetAsync()
    {
        if (SelectedProjet == null) return;

        await _projetService.ArchiverProjetAsync(SelectedProjet.Id);
        await LoadProjetsAsync();
    }
}