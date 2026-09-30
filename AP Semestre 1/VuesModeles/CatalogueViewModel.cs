using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Modeles;
using AutoMasters.Services;

namespace AutoMasters.ViewModels;

public partial class CatalogueViewModel : BaseViewModel
{
    private readonly ServiceApi _api;
    public ObservableCollection<Vehicule> Vehicules { get; } = [];

    [ObservableProperty]
    private string recherche = "";

    [ObservableProperty]
    private Rarete? rareteSelectionnee;

    // On expose la liste des raretés pour le menu déroulant XAML
    public List<Rarete?> FiltresRarete { get; } =
    [
        null, // Correspond à "Toutes"
        Rarete.Commune,
        Rarete.PeuCommune,
        Rarete.Rare,
        Rarete.Epique,
        Rarete.Legendaire,
        Rarete.Mythique
    ];

    public CatalogueViewModel(ServiceApi? serviceApi = null)
    {
        _api = serviceApi ?? ServiceApi.Instance;
        // On lance le chargement au démarrage
        _ = ChargerAsync();
    }

    [RelayCommand]
    public async Task ChargerAsync()
    {
        // J'ai supprimé la condition qui bloquait le chargement initial.

        try
        {
            State = AutoMasters.Modeles.ViewState.Loading;
            Vehicules.Clear();

            // Simulation de délai réseau pour voir le loader
            await Task.Delay(500);

            var resultats = await _api.GetVehiculesAsync(Recherche, RareteSelectionnee);
            foreach (var v in resultats)
            {
                Vehicules.Add(v);
            }

            State = Vehicules.Count == 0 ? AutoMasters.Modeles.ViewState.Empty : AutoMasters.Modeles.ViewState.Content;
        }
        catch
        {
            State = AutoMasters.Modeles.ViewState.Offline;
        }
    }
}
