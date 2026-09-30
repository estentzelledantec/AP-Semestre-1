using AutoMasters.ViewModels;
using AutoMasters.Services;
namespace AutoMasters.Vues;
public partial class AccueilPage : ContentPage
{
    public AccueilPage() : this(new AccueilViewModel(ServiceApi.Instance)) { }

    public AccueilPage(AccueilViewModel vm) { InitializeComponent(); BindingContext=vm; }
}
