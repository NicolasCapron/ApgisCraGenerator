using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ApgisCraGenerator.Models;
using ApgisCraGenerator.Services;

namespace ApgisCraGenerator.ViewModels;

public partial class CraViewModel : ObservableObject
{
    private readonly TacheService _tacheService;
    private readonly CraExportService _craExportService;

    [ObservableProperty]
    private int _mois = DateTime.Now.Month;

    [ObservableProperty]
    private int _annee = DateTime.Now.Year;

    [ObservableProperty]
    private ObservableCollection<Tache> _tachesMois = new();

    [ObservableProperty]
    private decimal _totalPourcentage;

    [ObservableProperty]
    private string _craTexte = string.Empty;

    [ObservableProperty]
    private bool _isValid;

    public CraViewModel(TacheService tacheService, CraExportService craExportService)
    {
        _tacheService = tacheService;
        _craExportService = craExportService;
        _ = ChargerDonneesAsync();
    }

    [RelayCommand]
    private async Task ChargerDonneesAsync()
    {
        var taches = await _tacheService.GetTachesAsync(Mois, Annee, null, null);
        TachesMois.Clear();
        foreach (var tache in taches)
        {
            TachesMois.Add(tache);
        }

        TotalPourcentage = await _tacheService.GetTotalPourcentageAsync(Mois, Annee);
        IsValid = Math.Abs(TotalPourcentage - 100) < 0.01m;
    }

    [RelayCommand]
    private async Task GenererCraAsync()
    {
        CraTexte = await _craExportService.GenererCraAsync(Mois, Annee);
    }

    [RelayCommand]
    private void CopierCra()
    {
        if (string.IsNullOrEmpty(CraTexte))
        {
            MessageBox.Show("Veuillez d'abord générer le CRA.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _craExportService.CopierDansPressePapier(CraTexte);
        MessageBox.Show("CRA copié dans le presse-papier !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task ExporterCraAsync()
    {
        if (string.IsNullOrEmpty(CraTexte))
        {
            MessageBox.Show("Veuillez d'abord générer le CRA.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        await _craExportService.ExporterVersFichierAsync(CraTexte, Mois, Annee);
        MessageBox.Show("CRA exporté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    partial void OnMoisChanged(int value) => _ = ChargerDonneesAsync();
    partial void OnAnneeChanged(int value) => _ = ChargerDonneesAsync();
}