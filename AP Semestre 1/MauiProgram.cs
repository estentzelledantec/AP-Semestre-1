using Microsoft.Extensions.Logging;
using AutoMasters.Services;
using AutoMasters.ViewModels;
using AutoMasters.Vues;

namespace AutoMasters
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(ServiceApi.Instance);
            builder.Services.AddTransient<AccueilViewModel>();
            builder.Services.AddTransient<BoosterViewModel>();
            builder.Services.AddTransient<CatalogueViewModel>();
            builder.Services.AddTransient<ConnexionViewModel>();
            builder.Services.AddTransient<VehiculeViewModel>();
            builder.Services.AddTransient<MarcheViewModel>();
            builder.Services.AddTransient<AccueilPage>();
            builder.Services.AddTransient<BoosterPage>();
            builder.Services.AddTransient<CataloguePage>();
            builder.Services.AddTransient<ConnexionPage>();
            builder.Services.AddTransient<MarchePage>();
            builder.Services.AddTransient<VehiculePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
