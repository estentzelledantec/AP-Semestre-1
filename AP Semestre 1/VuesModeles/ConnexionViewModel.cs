using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMasters.Services;

namespace AutoMasters.ViewModels;

public partial class ConnexionViewModel : BaseViewModel
{
    private readonly ServiceApi _serviceApi;

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string motDePasse = "";

    public bool AfficheErreur => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ConnexionViewModel(ServiceApi? serviceApi = null)
    {
        _serviceApi = serviceApi ?? ServiceApi.Instance;
        State = AutoMasters.Modeles.ViewState.Content;
    }

    [RelayCommand]
    public async Task SeConnecterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            OnPropertyChanged(nameof(AfficheErreur));
            return;
        }

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ErrorMessage = "Le format de l'adresse email est invalide.";
            OnPropertyChanged(nameof(AfficheErreur));
            return;
        }

        State = AutoMasters.Modeles.ViewState.Loading;
        ErrorMessage = "";
        OnPropertyChanged(nameof(AfficheErreur));

        try
        {
            // Le serveur reçoit le mot de passe au format attendu par son endpoint.
            await _serviceApi.SeConnecterAsync(Email.Trim(), MotDePasse);
            Email = "";
            MotDePasse = "";
            await Shell.Current.GoToAsync("//home/MainPage");
        }
        catch (Exception erreur)
        {
            ErrorMessage = erreur.Message;
            OnPropertyChanged(nameof(AfficheErreur));
            State = AutoMasters.Modeles.ViewState.Content;
        }
    }
}
