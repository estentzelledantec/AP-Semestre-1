using System.Collections.ObjectModel;
using AutoMasters.Modeles;
namespace AutoMasters.ViewModels;
public sealed class MarcheViewModel : BaseViewModel
{
    public ObservableCollection<Enchere> Encheres { get; } = [];
    public MarcheViewModel()
    {
        State=AutoMasters.Modeles.ViewState.Content;
        Encheres.Add(new Enchere { Vendeur="Collector01", PrixActuel=215000, NombreEncheres=7, Fin=DateTimeOffset.UtcNow.AddMinutes(12),
            Carte=new Carte { Vehicule=new Vehicule { Marque="Nissan", Modele="Skyline GT-R R34", Motorisation="RB26DETT", Rarete=Rarete.Legendaire, Prix=220000, ImageUrl="https://images.unsplash.com/photo-1552519507-da3b142c6e3d?auto=format&fit=crop&w=1000&q=80" } }});
    }
}
