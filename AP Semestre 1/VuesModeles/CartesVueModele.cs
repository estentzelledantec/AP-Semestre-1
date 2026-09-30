using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Modeles;
using AutoMasters.Services;
using AutoMasters.Vues;

namespace AutoMasters.VuesModeles;

public partial class CartesVueModele : ObservableObject
{
    #region Attributs

    private ServiceApi serviceApi = ServiceApi.Instance;

    [ObservableProperty]
    private ObservableCollection<CartePossedee> cartesPossedees
        = new ObservableCollection<CartePossedee>();

    [ObservableProperty]
    private CartePossedee? carteSelectionnee;

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstDisponible))]
    private bool estOccupe;

    #endregion

    #region Proprietes

    public bool EstDisponible => !EstOccupe;

    #endregion

    #region Commandes

    [RelayCommand]
    private async Task ChargerCartesAsync()
    {
        if (EstOccupe)
            return;

        EstOccupe = true;
        Message = "";
        try
        {
            CarteSelectionnee = null;
            CartesPossedees =
                await serviceApi.RecupererCartesPossedeesAsync();
            if (CartesPossedees.Count == 0)
                Message = "Votre compte ne possède aucune carte.";
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
    private async Task GererOffreAsync()
    {
        if (EstOccupe)
            return;
        if (CarteSelectionnee == null)
        {
            Message = "Sélectionnez une carte.";
            return;
        }

        EstOccupe = true;
        try
        {
            await Application.Current!.Windows[0].Page!
                .Navigation.PushAsync(
                    new OffrePage(CarteSelectionnee));
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
