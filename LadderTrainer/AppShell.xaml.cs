namespace LadderTrainer;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("workout", typeof(WorkoutPage));
    }
}
