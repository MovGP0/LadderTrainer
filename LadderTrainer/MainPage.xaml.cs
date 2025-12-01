using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

using Microsoft.Extensions.DependencyInjection;

namespace LadderTrainer;

public partial class MainPage : ReactiveContentPage<LadderSessionViewModel>
{
    // Parameterless constructor used when created from XAML/Shell; resolves ViewModel via DI.
    public MainPage() : this(App.Services.GetRequiredService<LadderSessionViewModel>())
    {
    }

    public MainPage(LadderSessionViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Share the same ViewModel instance with all child views
            SettingsView.ViewModel = ViewModel;
            WorkoutView.ViewModel = ViewModel;
            PauseView.ViewModel = ViewModel;
            StatisticsView.ViewModel = ViewModel;

            Disposable
                .Create(() => { /* teardown if needed */ })
                .DisposeWith(disposables);
        });
    }
}
