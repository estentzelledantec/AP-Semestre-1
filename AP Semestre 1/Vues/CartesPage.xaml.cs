using AutoMasters.VuesModeles;

namespace AutoMasters.Vues;

public partial class CartesPage : ContentPage
{
    #region Constructeurs

    public CartesPage()
    {
        InitializeComponent();
        BindingContext = new CartesVueModele();
    }

    #endregion


}
