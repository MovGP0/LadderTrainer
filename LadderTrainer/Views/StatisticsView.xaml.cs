using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Maui;

namespace LadderTrainer.Views;

public partial class StatisticsView : ReactiveContentView<LadderSessionViewModel>
{
    public StatisticsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel,
                    vm => vm.TargetRepetitions,
                    v  => v.TargetLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.CompletedRepetitions,
                    v  => v.CompletedLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.OverallTimeText,
                    v  => v.TotalTimeLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.IsStatisticsVisible,
                    v  => v.IsVisible)
                .DisposeWith(disposables);
        });
    }
}