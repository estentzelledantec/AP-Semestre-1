using AutoMasters.ViewModels;
namespace AutoMasters.Vues;
public partial class MarchePage : ContentPage
{
    public MarchePage() : this(new MarcheViewModel()) { }

    public MarchePage(MarcheViewModel vm) { InitializeComponent(); BindingContext=vm; }
}
