using LadderTrainer;
using Microsoft.Maui.Controls;

namespace LadderTrainer;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<LadderSessionViewModel>();

        if (BindingContext is LadderSessionViewModel vm)
        {
            vm.StartCommand.Subscribe(_ =>
            {
                if (Shell.Current is not null)
                {
                    Shell.Current.GoToAsync("workout");
                }
            });
        }
    }
}
