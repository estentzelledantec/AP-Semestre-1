using AutoMasters.ViewModels;

namespace AutoMasters.Vues;

public partial class CollectionPage : ContentPage
{
    public CollectionPage()
    {
        InitializeComponent();
        BindingContext = new CollectionViewModel();
    }
}