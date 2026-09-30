using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Modeles;
using AutoMasters.Services;

namespace AutoMasters.VuesModeles;

public partial class OffreVueModele : ObservableObject
{
    #region Attributs

    private ServiceApi serviceApi = ServiceApi.Instance;

    [ObservableProperty]
    private Offre? offreCourante;

    [ObservableProperty]
    private string prixSaisi = "";

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstDisponible))]
    private bool estOccupe;

    #endregion

    #region Constructeurs

    public OffreVueModele(CartePossedee cartePossedee)
    {
        CartePossedee = cartePossedee;
    }

    #endregion

    #region Proprietes

    public CartePossedee CartePossedee { get; }
    public bool EstDisponible => !EstOccupe;

    #endregion

    #region Methodes

    private decimal LirePrix()
    {
        string texte = PrixSaisi.Trim().Replace(',', '.');
        bool valide = decimal.TryParse(texte,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out decimal prix);

        if (!valide || prix <= 0)
            throw new Exception("Saisissez un prix positif.");

        return prix;
    }

    #endregion

    #region Commandes

    [RelayCommand]
    private async Task ChargerOffreAsync()
    {
        if (EstOccupe)
            return;
        EstOccupe = true;
        Message = "";
        try
        {
            OffreCourante = await serviceApi
                .RecupererOffreAsync(CartePossedee);
            if (OffreCourante == null)
            {
                PrixSaisi = "";
                Message = "Aucune offre : saisissez un prix.";
            }
            else
            {
                PrixSaisi = OffreCourante.Prix.ToString(
                    CultureInfo.CurrentCulture);
                Message = "Offre retrouvée.";
            }
        }
        catch (Exception erreur)
        {
            Message = erreur.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    [RelayCommand]
    private async Task CreerOffreAsync()
    {
        if (EstOccupe)
            return;
        EstOccupe = true;
        Message = "";
        try
        {
            if (OffreCourante != null)
                throw new Exception("Une offre existe déjà.");
            decimal prix = LirePrix();
            OffreCourante = await serviceApi.CreerOffreAsync(
                CartePossedee.Identifiant, prix);
            Message = "L'offre a été créée.";
        }
        catch (Exception erreur)
        {
            Message = erreur.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    [RelayCommand]
    private async Task ModifierOffreAsync()
    {
        if (EstOccupe)
            return;
        EstOccupe = true;
        Message = "";
        try
        {
            if (OffreCourante == null)
                throw new Exception("Aucune offre à modifier.");
            decimal prix = LirePrix();
            OffreCourante = await serviceApi.ModifierOffreAsync(
                OffreCourante.Identifiant, prix);
            Message = "L'offre a été modifiée.";
        }
        catch (Exception erreur)
        {
            Message = erreur.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    [RelayCommand]
    private async Task SupprimerOffreAsync()
    {
        if (EstOccupe)
            return;
        EstOccupe = true;
        Message = "";
        try
        {
            if (OffreCourante == null)
                throw new Exception("Aucune offre à supprimer.");
            await serviceApi.SupprimerOffreAsync(
                OffreCourante.Identifiant);
            OffreCourante = null;
            PrixSaisi = "";
            Message = "L'offre a été supprimée.";
        }
        catch (Exception erreur)
        {
            Message = erreur.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    #endregion
}
