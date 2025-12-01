using Microsoft.Extensions.Logging;
using ReactiveUI;
using Splat;

namespace LadderTrainer;

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

        // Dependency injection
        builder.Services.AddSingleton<LadderSessionViewModel>();
        builder.Services.AddSingleton<MainPage>();

        // Register ReactiveUI platform services so WhenActivated fires on MAUI views.
        Locator.CurrentMutable.InitializeReactiveUI();

        return builder.Build();
    }
}
