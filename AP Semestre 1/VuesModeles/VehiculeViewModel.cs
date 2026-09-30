using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Modeles;
using AutoMasters.Services;

namespace AutoMasters.ViewModels;

[QueryProperty(nameof(Vehicule), "VehiculeDetail")]
public partial class VehiculeViewModel : BaseViewModel
{
    private readonly ServiceApi _signalR;

    [ObservableProperty]
    private Vehicule? vehicule;

    // Propriété pour animer le fond du prix
    [ObservableProperty]
    private Color couleurPrix = Colors.Transparent;

    public VehiculeViewModel(ServiceApi? serviceApi = null)
    {
        _signalR = serviceApi ?? ServiceApi.Instance;
        State = AutoMasters.Modeles.ViewState.Loading;

        // On écoute le marché en temps réel
        _signalR.OnQuoteUpdated((vehiculeId, variationPrix) =>
        {
            if (Vehicule != null && Vehicule.Id == vehiculeId)
            {
                Vehicule.Prix += variationPrix;
                OnPropertyChanged(nameof(Vehicule)); // Force l'UI à afficher le nouveau prix

                FlashPrix(variationPrix > 0); // Effet visuel
            }
        });
    }

    partial void OnVehiculeChanged(Vehicule? value)
    {
        if (value is not null)
        {
            State = AutoMasters.Modeles.ViewState.Content;

            // On connecte SignalR quand on ouvre une fiche
            if (!_signalR.Connecte) _ = _signalR.ConnectAsync();
        }
    }

    // Animation de couleur (Vert = Hausse, Rouge = Baisse)
    private async void FlashPrix(bool hausse)
    {
        CouleurPrix = hausse ? Color.FromArgb("#22c55e") : Color.FromArgb("#ef4444");
        await Task.Delay(400);
        CouleurPrix = Colors.Transparent;
    }

    [RelayCommand]
    public async Task AcheterImmediatementAsync()
    {
        if (Vehicule != null)
        {
            // Nouvelle syntaxe MAUI pour les alertes (corrige l'avertissement CS0618 et CS8602)
            await Shell.Current.DisplayAlertAsync("Achat", $"Vous venez d'acquérir la {Vehicule.Marque} {Vehicule.Modele} !", "Génial");
        }
    }
}
