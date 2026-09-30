using AutoMasters.VuesModeles;
using AutoMasters.Modeles;

namespace AutoMasters.Vues;

public partial class OffrePage : ContentPage
{
    #region Constructeurs

    public OffrePage(CartePossedee cartePossedee)
    {
        InitializeComponent();
        BindingContext = new OffreVueModele(cartePossedee);
    }

    #endregion


}
