using AutoMasters.ViewModels;
using AutoMasters.Services;
namespace AutoMasters.Vues;
public partial class VehiculePage : ContentPage
{
    public VehiculePage() : this(new VehiculeViewModel(ServiceApi.Instance)) { }

    public VehiculePage(VehiculeViewModel vm) { InitializeComponent(); BindingContext=vm; }
}
