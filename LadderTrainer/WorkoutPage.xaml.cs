using LadderTrainer;
using Microsoft.Maui.Controls;

namespace LadderTrainer;

public partial class WorkoutPage : ContentPage
{
    public WorkoutPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<LadderSessionViewModel>();
    }
}
