using Microsoft.Extensions.DependencyInjection;

namespace LadderTrainer;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public App(IServiceProvider services)
    {
        Services = services;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Use Shell so navigation can be expanded later.
        return new Window(new AppShell());
    }
}
