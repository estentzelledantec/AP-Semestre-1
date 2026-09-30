using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Modeles;
using AutoMasters.Services;

namespace AutoMasters.ViewModels;

[QueryProperty(nameof(AutoOuverture), "AutoOuverture")]
public partial class BoosterViewModel : BaseViewModel
{
    private readonly ServiceApi _service;
    private readonly IDispatcherTimer _timer;

    public ObservableCollection<Carte> Cartes { get; } = [];

    [ObservableProperty] private int index = -1;
    [ObservableProperty] private string compteur = "";
    [ObservableProperty] private bool fini;
    [ObservableProperty] private bool autoOuverture;

    public Carte? CarteActuelle => Index >= 0 && Index < Cartes.Count ? Cartes[Index] : null;
    public bool HasCurrentCard => CarteActuelle is not null;
    public bool IsReadyToOpen => Cartes.Count == 0 || Fini;

    // Nouvelles variables pour gérer l'affichage des flèches et du bouton Terminer
    public string TexteProgression => $"Carte {Index + 1} sur {Cartes.Count}";
    public bool PeutAllerPrecedent => Index > 0;
    public bool NEstPasDerniereCarte => Index >= 0 && Index < Cartes.Count - 1;
    public bool EstDerniereCarte => Index == Cartes.Count - 1;

    public BoosterViewModel(ServiceApi? serviceApi = null)
    {
        _service = serviceApi ?? ServiceApi.Instance;
        State = AutoMasters.Modeles.ViewState.Content;

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => Actualiser();
        _timer.Start();

        Actualiser();
    }

    partial void OnAutoOuvertureChanged(bool value)
    {
        if (value)
        {
            AutoOuverture = false;
            if (IsReadyToOpen && _service.PeutOuvrir)
            {
                _ = OuvrirAsync();
            }
        }
    }

    [RelayCommand]
    public async Task OuvrirAsync()
    {
        if (!_service.PeutOuvrir || State == AutoMasters.Modeles.ViewState.Loading) return;

        try
        {
            State = AutoMasters.Modeles.ViewState.Loading;
            Index = -1;
            RafraichirProprietes();
            Cartes.Clear();

            await Task.Delay(1500);
            var nouvellesCartes = await _service.OuvrirAsync();

            var cartesMelangees = nouvellesCartes.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var c in cartesMelangees) Cartes.Add(c);

            // Dès l'ouverture, la première carte est directement affichée
            Index = 0;
            Fini = false;

            RafraichirProprietes();
            State = AutoMasters.Modeles.ViewState.Content;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            State = AutoMasters.Modeles.ViewState.Error;
        }
    }

    // Ces méthodes sont maintenant appelées par le Code-Behind après l'animation
    public void PasserCarteSuivante()
    {
        if (Index < Cartes.Count - 1)
        {
            Index++;
            RafraichirProprietes();
        }
    }

    public void PasserCartePrecedente()
    {
        if (Index > 0)
        {
            Index--;
            RafraichirProprietes();
        }
    }

    public async Task TerminerOuvertureAsync()
    {
        Fini = true;
        Index = -1;
        Cartes.Clear();
        RafraichirProprietes();

        await Shell.Current.GoToAsync("//home/MainPage");
    }

    public void RafraichirProprietes()
    {
        OnPropertyChanged(nameof(CarteActuelle));
        OnPropertyChanged(nameof(HasCurrentCard));
        OnPropertyChanged(nameof(IsReadyToOpen));
        OnPropertyChanged(nameof(TexteProgression));
        OnPropertyChanged(nameof(PeutAllerPrecedent));
        OnPropertyChanged(nameof(NEstPasDerniereCarte));
        OnPropertyChanged(nameof(EstDerniereCarte));
    }

    public void Actualiser()
    {
        _service.Tick();
        Compteur = _service.PeutOuvrir
            ? $"{_service.Etat.Stock}/10 paquets disponibles"
            : $"Prochain dans {_service.TempsRestant:mm\\:ss}";
    }
}
