namespace AutoMasters
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Vues.BoosterPage), typeof(Vues.BoosterPage));
            Routing.RegisterRoute(nameof(Vues.VehiculePage), typeof(Vues.VehiculePage));
        }
    }
}
