using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Maui;

namespace LadderTrainer.Views;

public partial class WorkoutView : ReactiveContentView<LadderSessionViewModel>
{
    public WorkoutView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Take(1)
                .Subscribe(vm =>
                {
                    this.OneWayBind(vm!,
                            x => x.CurrentSetRepetitions,
                            v  => v.CurrentRepsLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.CompletedRepetitions,
                            v  => v.CompletedRepsLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.RemainingRepetitions,
                            v  => v.RemainingRepsLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.CurrentPhaseLabel,
                            v  => v.PhaseLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.OverallTimeText,
                            v  => v.OverallTimeLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.CurrentPhaseTimeText,
                            v  => v.CurrentPhaseTimeLabel.Text)
                        .DisposeWith(disposables);

                    this.BindCommand(vm!,
                            x => x.SetDoneCommand,
                            v  => v.SetDoneButton)
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
