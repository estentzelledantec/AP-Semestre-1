using AutoMasters.ViewModels;
using AutoMasters.Modeles;
namespace AutoMasters.Vues;
public partial class CataloguePage : ContentPage
{
    public CataloguePage() : this(new CatalogueViewModel(AutoMasters.Services.ServiceApi.Instance)) { }

    public CataloguePage(CatalogueViewModel vm) { InitializeComponent(); BindingContext=vm; }

    private async void OnVehiculeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Vehicule vehicule)
            return;

        if (sender is CollectionView collection)
            collection.SelectedItem = null;

        await Shell.Current.GoToAsync(nameof(VehiculePage), new Dictionary<string, object>
        {
            ["VehiculeDetail"] = vehicule
        });
    }
}
