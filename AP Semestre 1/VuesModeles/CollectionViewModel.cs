using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoMasters.ViewModels;

public class CarteCollectionModel
{
    public string Marque { get; set; } = "";
    public string Modele { get; set; } = "";
    public string Categorie { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string RareteCode { get; set; } = ""; // L, UR, SR, R, PC, C
    public int PoidsRarete { get; set; } // Pour trier du plus rare au moins rare

    public Color CouleurRarete => RareteCode switch
    {
        "L" => Color.FromArgb("#ef4444"),  // Rouge
        "UR" => Color.FromArgb("#a855f7"), // Violet
        "SR" => Color.FromArgb("#3b82f6"), // Bleu
        "R" => Color.FromArgb("#22c55e"),  // Vert
        "PC" => Color.FromArgb("#ffffff"), // Blanc
        "C" => Color.FromArgb("#9ca3af"),  // Gris
        _ => Color.FromArgb("#333333")
    };
}

public partial class CollectionViewModel : BaseViewModel
{
    private List<CarteCollectionModel> _toutesLesCartes = new();

    // Collection affichée (filtrée et paginée)
    public ObservableCollection<CarteCollectionModel> CartesFiltrees { get; } = new();

    [ObservableProperty] private string texteRecherche = "";
    [ObservableProperty] private string pageInfo = "Page 1 / 1";
    [ObservableProperty] private bool aDesResultats = true;
    [ObservableProperty] private bool pasDeResultat;

    // États de sélection des boutons de rareté
    [ObservableProperty] private bool estTousSelectionne = true;
    [ObservableProperty] private bool estLSelected;
    [ObservableProperty] private bool estURSelected;
    [ObservableProperty] private bool estSRSelected;
    [ObservableProperty] private bool estRSelected;
    [ObservableProperty] private bool estPCSelected;
    [ObservableProperty] private bool estCSelected;

    // Gestion de la pagination
    private int _pageActuelle = 1;
    private const int _elementsParPage = 4; // Tu pourras monter à 10 ou 20 plus tard
    private int _totalPages = 1;

    public CollectionViewModel()
    {
        InitialiserDonnees();
        AppliquerFiltres();
    }

    private void InitialiserDonnees()
    {
        // Poids : plus le chiffre est haut, plus c'est rare (pour le tri du plus rare au moins rare)
        _toutesLesCartes = new List<CarteCollectionModel>
        {
            new() { Marque = "Ferrari", Modele = "LaFerrari", Categorie = "Hypercar", RareteCode = "L", PoidsRarete = 6, ImageUrl = "https://images.unsplash.com/photo-1592198084033-aade902d1aae?auto=format&fit=crop&w=500" },
            new() { Marque = "Bugatti", Modele = "Chiron", Categorie = "Hypercar", RareteCode = "L", PoidsRarete = 6, ImageUrl = "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?auto=format&fit=crop&w=500" },
            new() { Marque = "Porsche", Modele = "911 GT3 RS", Categorie = "Sport", RareteCode = "UR", PoidsRarete = 5, ImageUrl = "https://images.unsplash.com/photo-1503376712341-ea1c64dfc4a9?auto=format&fit=crop&w=500" },
            new() { Marque = "Lamborghini", Modele = "Huracan", Categorie = "Supercar", RareteCode = "SR", PoidsRarete = 4, ImageUrl = "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?auto=format&fit=crop&w=500" },
            new() { Marque = "BMW", Modele = "M4 Compétition", Categorie = "Sport", RareteCode = "R", PoidsRarete = 3, ImageUrl = "https://images.unsplash.com/photo-1555353540-64fd8b0222f7?auto=format&fit=crop&w=500" },
            new() { Marque = "Toyota", Modele = "GR Yaris", Categorie = "Citadine", RareteCode = "PC", PoidsRarete = 2, ImageUrl = "https://images.unsplash.com/photo-1629897048514-3dd7415d4838?auto=format&fit=crop&w=500" },
            new() { Marque = "Renault", Modele = "Clio V", Categorie = "Citadine", RareteCode = "C", PoidsRarete = 1, ImageUrl = "https://images.unsplash.com/photo-1620882191565-9856f63be74c?auto=format&fit=crop&w=500" }
        };
    }

    // Déclenché à chaque frappe dans la barre de recherche
    partial void OnTexteRechercheChanged(string value)
    {
        _pageActuelle = 1;
        AppliquerFiltres();
    }

    [RelayCommand]
    public void BasculerTous()
    {
        EstTousSelectionne = true;
        EstLSelected = false;
        EstURSelected = false;
        EstSRSelected = false;
        EstRSelected = false;
        EstPCSelected = false;
        EstCSelected = false;

        _pageActuelle = 1;
        AppliquerFiltres();
    }

    [RelayCommand]
    public void BasculerFiltre(string rarete)
    {
        // Si l'utilisateur clique sur une rareté, on désactive le mode "Tous"
        EstTousSelectionne = false;

        switch (rarete)
        {
            case "L": EstLSelected = !EstLSelected; break;
            case "UR": EstURSelected = !EstURSelected; break;
            case "SR": EstSRSelected = !EstSRSelected; break;
            case "R": EstRSelected = !EstRSelected; break;
            case "PC": EstPCSelected = !EstPCSelected; break;
            case "C": EstCSelected = !EstCSelected; break;
        }

        // Si plus aucune rareté n'est cochée, on réactive "Tous" automatiquement
        if (!EstLSelected && !EstURSelected && !EstSRSelected && !EstRSelected && !EstPCSelected && !EstCSelected)
        {
            EstTousSelectionne = true;
        }

        _pageActuelle = 1;
        AppliquerFiltres();
    }

    private void AppliquerFiltres()
    {
        var resultat = _toutesLesCartes.AsEnumerable();

        // 1. Filtre de recherche intelligente (Marque ou Modèle)
        if (!string.IsNullOrWhiteSpace(TexteRecherche))
        {
            var recherche = TexteRecherche.Trim().ToLower();
            resultat = resultat.Where(c => c.Marque.ToLower().Contains(recherche) || c.Modele.ToLower().Contains(recherche));
        }

        // 2. Filtre par raretés sélectionnées (si "Tous" n'est pas actif)
        if (!EstTousSelectionne)
        {
            var raretésActives = new List<string>();
            if (EstLSelected) raretésActives.Add("L");
            if (EstURSelected) raretésActives.Add("UR");
            if (EstSRSelected) raretésActives.Add("SR");
            if (EstRSelected) raretésActives.Add("R");
            if (EstPCSelected) raretésActives.Add("PC");
            if (EstCSelected) raretésActives.Add("C");

            resultat = resultat.Where(c => raretésActives.Contains(c.RareteCode));
        }

        // 3. Tri intelligent demandé : du plus rare au moins rare (Poids décroissant)
        resultat = resultat.OrderByDescending(c => c.PoidsRarete);

        var listeFiltree = resultat.ToList();

        // Gestion de la pagination
        _totalPages = Math.Max(1, (int)Math.Ceiling(listeFiltree.Count / (double)_elementsParPage));
        if (_pageActuelle > _totalPages) _pageActuelle = _totalPages;

        var elementsPage = listeFiltree
            .Skip((_pageActuelle - 1) * _elementsParPage)
            .Take(_elementsParPage)
            .ToList();

        CartesFiltrees.Clear();
        foreach (var c in elementsPage)
        {
            CartesFiltrees.Add(c);
        }

        PageInfo = $"Page {_pageActuelle} / {_totalPages}";
        ADesResultats = CartesFiltrees.Count > 0;
        PasDeResultat = !ADesResultats;
    }

    [RelayCommand]
    public void PagePrecedente()
    {
        if (_pageActuelle > 1)
        {
            _pageActuelle--;
            AppliquerFiltres();
        }
    }

    [RelayCommand]
    public void PageSuivante()
    {
        if (_pageActuelle < _totalPages)
        {
            _pageActuelle++;
            AppliquerFiltres();
        }
    }
}