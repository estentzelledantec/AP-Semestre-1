using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AutoMasters.Modeles;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace AutoMasters.Services;

/// <summary>
/// Point d'accès unique aux données, à l'authentification, aux boosters,
/// aux cours des véhicules et aux variations de prix.
/// </summary>
public sealed class ServiceApi
{
    private const string TokenKey = "automasters_jwt";
    private static readonly Lazy<ServiceApi> InstanceLazy = new(() => new ServiceApi());
    private readonly HttpClient _client;
    private readonly List<Vehicule> _vehicules =
    [
        new() { Id = 1, Marque = "Toyota", Modele = "Supra MK4", Motorisation = "2JZ-GTE 3.0 Twin Turbo", Annee = 1998, PuissanceCh = 330, Production = 16400, Prix = 85000, Rarete = Rarete.Rare, ImageUrl = "https://images.unsplash.com/photo-1619767886558-efdc259cde1a?auto=format&fit=crop&w=1000&q=80", Description = "Icône japonaise de la génération JDM." },
        new() { Id = 2, Marque = "Nissan", Modele = "Skyline GT-R R34", Motorisation = "RB26DETT 2.6 Twin Turbo", Annee = 2002, PuissanceCh = 280, Production = 11378, Prix = 220000, Rarete = Rarete.Legendaire, ImageUrl = "https://images.unsplash.com/photo-1552519507-da3b142c6e3d?auto=format&fit=crop&w=1000&q=80", Description = "L'une des GT japonaises les plus recherchées." },
        new() { Id = 3, Marque = "Ferrari", Modele = "F40", Motorisation = "V8 2.9 Twin Turbo", Annee = 1990, PuissanceCh = 478, Production = 1311, Prix = 1900000, Rarete = Rarete.Mythique, ImageUrl = "https://images.unsplash.com/photo-1592198084033-aade902d1aae?auto=format&fit=crop&w=1000&q=80", Description = "Supercar emblématique de Ferrari." },
        new() { Id = 4, Marque = "BMW", Modele = "M3 E46", Motorisation = "S54 3.2 I6", Annee = 2005, PuissanceCh = 343, Production = 85000, Prix = 55000, Rarete = Rarete.PeuCommune, ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=1000&q=80", Description = "Référence des sportives compactes des années 2000." },
        new() { Id = 5, Marque = "Renault", Modele = "Clio 2 1.2", Motorisation = "1.2 16V", Annee = 2004, PuissanceCh = 75, Production = 900000, Prix = 3500, Rarete = Rarete.Commune, ImageUrl = "https://images.unsplash.com/photo-1502877338535-766e1452684a?auto=format&fit=crop&w=1000&q=80", Description = "Modèle courant et accessible." }
    ];

    private CancellationTokenSource? _marcheCancellation;
    private Action<int, decimal>? _onQuoteUpdated;

    private ServiceApi()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://api-cartes-g1.fldsio.com/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
        Etat = new Booster { Stock = 10, StockMax = 10 };
    }

    public static ServiceApi Instance => InstanceLazy.Value;

    #region Authentification et API des cartes

    private async Task<string> LireReponseAsync(HttpResponseMessage reponse)
    {
        string json = await reponse.Content.ReadAsStringAsync();

        if (!reponse.IsSuccessStatusCode)
        {
            ErreurApi? erreur = null;
            try
            {
                erreur = JsonConvert.DeserializeObject<ErreurApi>(json);
            }
            catch (JsonException)
            {
                // L'API peut renvoyer une erreur qui n'est pas au format JSON.
            }

            if (reponse.StatusCode == HttpStatusCode.Unauthorized)
                throw new InvalidOperationException(
                    "Connexion refusée ou expirée. Reconnectez-vous.");

            throw new InvalidOperationException(erreur?.Message ??
                "Le serveur ne peut pas traiter la demande.");
        }

        return json;
    }

    private async Task RestaurerAuthentificationAsync()
    {
        if (_client.DefaultRequestHeaders.Authorization is null)
            await GetTokenAsync();
    }

    public async Task SeConnecterAsync(string identifiant, string motDePasse)
    {
        var demande = new DemandeConnexion
        {
            Identifiant = identifiant,
            MotDePasse = motDePasse
        };
        using var contenu = new StringContent(
            JsonConvert.SerializeObject(demande), Encoding.UTF8, "application/json");
        using HttpResponseMessage reponse = await _client.PostAsync("auth/login", contenu);
        string resultat = await LireReponseAsync(reponse);
        ReponseConnexion? connexion = JsonConvert.DeserializeObject<ReponseConnexion>(resultat);

        if (connexion is null || string.IsNullOrWhiteSpace(connexion.JetonAcces))
            throw new InvalidOperationException("Le jeton est absent.");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", connexion.JetonAcces);
        await SecureStorage.Default.SetAsync(TokenKey, connexion.JetonAcces);
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        try
        {
            await SeConnecterAsync(email.Trim(), password);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task LogoutAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task<string?> GetTokenAsync()
    {
        string? token = await SecureStorage.Default.GetAsync(TokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return token;
    }

    public async Task<ObservableCollection<CartePossedee>> RecupererCartesPossedeesAsync()
    {
        await RestaurerAuthentificationAsync();
        using HttpResponseMessage reponse = await _client.GetAsync("portfolio");
        string json = await LireReponseAsync(reponse);
        Portefeuille? portefeuille = JsonConvert.DeserializeObject<Portefeuille>(json);

        if (portefeuille is null)
            throw new InvalidOperationException("Le portefeuille est absent.");

        return portefeuille.CartesPossedees;
    }

    public async Task<Offre?> RecupererOffreAsync(CartePossedee cartePossedee)
    {
        await RestaurerAuthentificationAsync();
        string adresse = $"cards/{Uri.EscapeDataString(cartePossedee.Carte.Identifiant)}/offers";
        using HttpResponseMessage reponse = await _client.GetAsync(adresse);
        string json = await LireReponseAsync(reponse);
        ReponseOffres? resultat = JsonConvert.DeserializeObject<ReponseOffres>(json);

        if (resultat is null)
            throw new InvalidOperationException("Les offres sont absentes.");

        return resultat.Offres.FirstOrDefault(offre =>
            offre.IdentifiantExemplaire == cartePossedee.Identifiant);
    }

    public async Task<Offre> CreerOffreAsync(string identifiantExemplaire, decimal prix)
    {
        await RestaurerAuthentificationAsync();
        var demande = new DemandeCreationOffre
        {
            IdentifiantExemplaire = identifiantExemplaire,
            Prix = prix
        };
        using var contenu = new StringContent(
            JsonConvert.SerializeObject(demande), Encoding.UTF8, "application/json");
        using HttpResponseMessage reponse = await _client.PostAsync("offers", contenu);
        string resultat = await LireReponseAsync(reponse);
        return JsonConvert.DeserializeObject<Offre>(resultat)
            ?? throw new InvalidOperationException("L'offre créée est absente.");
    }

    public async Task<Offre> ModifierOffreAsync(string identifiantOffre, decimal prix)
    {
        await RestaurerAuthentificationAsync();
        var demande = new DemandeModificationOffre { Prix = prix };
        using var requete = new HttpRequestMessage(
            HttpMethod.Patch, $"offers/{Uri.EscapeDataString(identifiantOffre)}")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(demande), Encoding.UTF8, "application/json")
        };
        using HttpResponseMessage reponse = await _client.SendAsync(requete);
        string resultat = await LireReponseAsync(reponse);
        return JsonConvert.DeserializeObject<Offre>(resultat)
            ?? throw new InvalidOperationException("L'offre modifiée est absente.");
    }

    public async Task SupprimerOffreAsync(string identifiantOffre)
    {
        await RestaurerAuthentificationAsync();
        using HttpResponseMessage reponse = await _client.DeleteAsync(
            $"offers/{Uri.EscapeDataString(identifiantOffre)}");
        await LireReponseAsync(reponse);
    }

    #endregion

    #region Catalogue et enchères de démonstration

    public IReadOnlyList<Vehicule> All => _vehicules;

    public Task<IReadOnlyList<Vehicule>> GetVehiculesAsync(
        string? recherche = null, Rarete? rarete = null)
    {
        IEnumerable<Vehicule> resultat = _vehicules;
        if (!string.IsNullOrWhiteSpace(recherche))
        {
            resultat = resultat.Where(vehicule =>
                $"{vehicule.Marque} {vehicule.Modele} {vehicule.Motorisation}"
                    .Contains(recherche.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (rarete.HasValue)
            resultat = resultat.Where(vehicule => vehicule.Rarete == rarete.Value);

        return Task.FromResult<IReadOnlyList<Vehicule>>(resultat.ToList());
    }

    public Task<IReadOnlyList<Enchere>> GetEncheresAsync() =>
        Task.FromResult<IReadOnlyList<Enchere>>([]);

    #endregion

    #region Boosters

    public Booster Etat { get; }

    public TimeSpan TempsRestant
    {
        get
        {
            if (Etat.ProchainDisponible is not { } prochain)
                return TimeSpan.Zero;

            TimeSpan restant = prochain - DateTimeOffset.UtcNow;
            return restant > TimeSpan.Zero ? restant : TimeSpan.Zero;
        }
    }

    public bool PeutOuvrir => Etat.Stock > 0;

    public Task<IReadOnlyList<Carte>> OuvrirAsync()
    {
        if (!PeutOuvrir)
            throw new InvalidOperationException("Aucun booster disponible.");

        Etat.Stock--;
        if (Etat.Stock < Etat.StockMax && Etat.ProchainDisponible is null)
            Etat.ProchainDisponible = DateTimeOffset.UtcNow.AddMinutes(10);

        List<Carte> cartes = Enumerable.Range(0, 5)
            .Select(_ =>
            {
                Vehicule vehicule = _vehicules[Random.Shared.Next(_vehicules.Count)];
                return new Carte { Vehicule = vehicule, PrixAchat = vehicule.Prix };
            })
            .ToList();

        Etat.Cartes = cartes;
        return Task.FromResult<IReadOnlyList<Carte>>(cartes);
    }

    public void StartTimer()
    {
        if (Etat.Stock < Etat.StockMax && Etat.ProchainDisponible is null)
            Etat.ProchainDisponible = DateTimeOffset.UtcNow.AddMinutes(10);
    }

    public void Tick()
    {
        if (Etat.ProchainDisponible is not { } prochain || prochain > DateTimeOffset.UtcNow)
            return;

        int paquetsRecuperes = 1 + (int)((DateTimeOffset.UtcNow - prochain).TotalMinutes / 10);
        Etat.Stock = Math.Min(Etat.StockMax, Etat.Stock + paquetsRecuperes);
        Etat.ProchainDisponible = Etat.Stock < Etat.StockMax
            ? prochain.AddMinutes(paquetsRecuperes * 10)
            : null;
    }

    #endregion

    #region Prix et variations du marché

    public decimal CalculerCote(Vehicule vehicule) => Math.Max(
        500,
        Math.Round(vehicule.Prix
            * (1 + (vehicule.Production < 5000 ? .18m : 0)
                + (DateTime.Now.Year - vehicule.Annee > 30 ? .12m : 0)), 0));

    public Rarete CalculerRarete(Vehicule vehicule)
    {
        if (vehicule.Prix >= 1_000_000 && vehicule.Production < 3_000)
            return Rarete.Mythique;
        if (vehicule.Prix >= 150_000 && vehicule.Production < 20_000)
            return Rarete.Legendaire;
        if (vehicule.Prix >= 50_000 && vehicule.Production < 100_000)
            return Rarete.Rare;
        if (vehicule.Prix >= 10_000)
            return Rarete.PeuCommune;
        return Rarete.Commune;
    }

    public bool Connecte { get; private set; }

    public void OnQuoteUpdated(Action<int, decimal> action)
    {
        _onQuoteUpdated += action;
    }

    public Task ConnectAsync()
    {
        if (Connecte)
            return Task.CompletedTask;

        Connecte = true;
        _marcheCancellation = new CancellationTokenSource();
        _ = SimulerMarcheAsync(_marcheCancellation.Token);
        return Task.CompletedTask;
    }

    private async Task SimulerMarcheAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                int idVehicule = Random.Shared.Next(1, _vehicules.Count + 1);
                decimal variation = Random.Shared.Next(-500, 500);
                MainThread.BeginInvokeOnMainThread(() =>
                    _onQuoteUpdated?.Invoke(idVehicule, variation));
            }
        }
        catch (OperationCanceledException)
        {
            // La simulation s'arrête proprement à la fermeture de l'application.
        }
    }

    #endregion
}
