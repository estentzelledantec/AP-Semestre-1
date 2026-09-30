using AutoMasters.ViewModels;
using AutoMasters.Services;

namespace AutoMasters.Vues;

public partial class ConnexionPage : ContentPage
{
    public ConnexionPage() : this(new ConnexionViewModel(ServiceApi.Instance)) { }

    public ConnexionPage(ConnexionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
