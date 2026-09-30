using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoMasters.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    // On précise explicitement "AutoMasters.Modeles.ViewState" partout
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoading))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(IsError))]
    [NotifyPropertyChangedFor(nameof(IsOffline))]
    private AutoMasters.Modeles.ViewState state = AutoMasters.Modeles.ViewState.Loading;

    [ObservableProperty]
    private string errorMessage = "";

    public bool IsLoading => State == AutoMasters.Modeles.ViewState.Loading;
    public bool IsEmpty => State == AutoMasters.Modeles.ViewState.Empty;
    public bool IsError => State == AutoMasters.Modeles.ViewState.Error;
    public bool IsOffline => State == AutoMasters.Modeles.ViewState.Offline;
}