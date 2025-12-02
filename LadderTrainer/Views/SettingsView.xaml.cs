using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Maui;
using Microsoft.Maui.Controls;

namespace LadderTrainer.Views;

public partial class SettingsView : ReactiveContentView<LadderSessionViewModel>
{
    public SettingsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Take(1)
                .Subscribe(vm =>
                {
                    // Two-way binding TargetRepetitions <-> Entry.Text
                    this.Bind(vm!,
                            x => x.TargetRepetitions,
                            v  => v.TargetEntry.Text)
                        .DisposeWith(disposables);

                    this.BindCommand(vm!,
                            x => x.StartCommand,
                            v  => v.StartButton)
                        .DisposeWith(disposables);

                    // Enable/disable and show/hide settings based on session phase
                    this.OneWayBind(vm!,
                            x => x.IsSettingsEnabled,
                            v  => v.IsEnabled)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.IsSettingsEnabled,
                            v  => v.IsVisible)
                        .DisposeWith(disposables);
                })
                .DisposeWith(disposables);
        });
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext as LadderSessionViewModel;
    }
}
