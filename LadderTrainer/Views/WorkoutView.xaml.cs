using System.Reactive.Disposables.Fluent;
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
            this.OneWayBind(ViewModel,
                    vm => vm.CurrentSetRepetitions,
                    v  => v.CurrentRepsLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.CompletedRepetitions,
                    v  => v.CompletedRepsLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.RemainingRepetitions,
                    v  => v.RemainingRepsLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.CurrentPhaseLabel,
                    v  => v.PhaseLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.OverallTimeText,
                    v  => v.OverallTimeLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.CurrentPhaseTimeText,
                    v  => v.CurrentPhaseTimeLabel.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.SetDoneCommand,
                    v  => v.SetDoneButton)
                .DisposeWith(disposables);

            // Show workout controls only during workout
            this.OneWayBind(ViewModel,
                    vm => vm.IsWorkoutActive,
                    v  => v.IsVisible)
                .DisposeWith(disposables);
        });
    }
}