using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Services;
using AutoMasters.Vues;

namespace AutoMasters.VuesModeles;

public partial class ConnexionVueModele : ObservableObject
{
    #region Attributs

    private ServiceApi serviceApi = ServiceApi.Instance;

    [ObservableProperty]
    private string identifiant = "";

    [ObservableProperty]
    private string motDePasse = "";

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
    private async Task SeConnecterAsync()
    {
        if (EstOccupe)
            return;

        if (string.IsNullOrWhiteSpace(Identifiant) ||
            string.IsNullOrWhiteSpace(MotDePasse))
        {
            Message = "Saisissez les deux identifiants.";
            return;
        }

        EstOccupe = true;
        Message = "";
        try
        {
            await serviceApi.SeConnecterAsync(
                Identifiant.Trim(), MotDePasse);
            MotDePasse = "";
            await Application.Current!.Windows[0].Page!
                .Navigation.PushAsync(new CartesPage());
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
