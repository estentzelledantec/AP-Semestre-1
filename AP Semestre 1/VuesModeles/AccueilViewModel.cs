using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Services;

namespace AutoMasters.ViewModels;

public partial class AccueilViewModel : BaseViewModel
{
    private readonly ServiceApi _boosters;
    private readonly IDispatcherTimer _timer;

    [ObservableProperty] private int stock;
    [ObservableProperty] private string texteDisponibilite = "";
    [ObservableProperty] private string tempsRestant = "";
    [ObservableProperty] private bool peutOuvrir;
    [ObservableProperty] private bool afficheTimer;

    // Collection pour dessiner les 10 points de la maquette
    public ObservableCollection<bool> PointsStock { get; } = new();

    public AccueilViewModel(ServiceApi? serviceApi = null)
    {
        _boosters = serviceApi ?? ServiceApi.Instance;
        State = AutoMasters.Modeles.ViewState.Content;

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => Actualiser();
        _timer.Start();

        InitialiserPoints();
        Actualiser();
    }

    private void InitialiserPoints()
    {
        for (int i = 0; i < 10; i++)
        {
            PointsStock.Add(false);
        }
    }

    public void Actualiser()
    {
        _boosters.Tick();
        Stock = _boosters.Etat.Stock;
        PeutOuvrir = _boosters.PeutOuvrir;

        TexteDisponibilite = $"{Stock}/10 disponibles";

        if (_boosters.TempsRestant > TimeSpan.Zero)
        {
            TempsRestant = $"Prochain dans {_boosters.TempsRestant:mm\\:ss}";
            AfficheTimer = true;
        }
        else
        {
            TempsRestant = "";
            AfficheTimer = false;
        }

        // Met à jour la couleur des 10 points (True = Rouge, False = Gris)
        for (int i = 0; i < 10; i++)
        {
            PointsStock[i] = (i < Stock);
        }
    }

    [RelayCommand]
    public async Task OuvrirBoosterAsync()
    {
        if (!PeutOuvrir) return;

        var parametresNavigation = new Dictionary<string, object>
        {
            { "AutoOuverture", true }
        };

        // CORRECTION ICI : on enlève les "//" car ce n'est plus un onglet
        await Shell.Current.GoToAsync(nameof(Vues.BoosterPage), parametresNavigation);
    }
}
