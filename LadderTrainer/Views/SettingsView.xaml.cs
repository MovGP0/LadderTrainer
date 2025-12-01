using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Maui;

namespace LadderTrainer.Views;

public partial class SettingsView : ReactiveContentView<LadderSessionViewModel>
{
    public SettingsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Two-way binding TargetRepetitions <-> Entry.Text
            // Default conversions (int <-> string) are used here.
            this.Bind(ViewModel,
                    vm => vm.TargetRepetitions,
                    v  => v.TargetEntry.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.StartCommand,
                    v  => v.StartButton)
                .DisposeWith(disposables);

            // Enable/disable settings based on session phase
            this.OneWayBind(ViewModel,
                    vm => vm.IsSettingsEnabled,
                    v  => v.IsEnabled)
                .DisposeWith(disposables);
        });
    }
}