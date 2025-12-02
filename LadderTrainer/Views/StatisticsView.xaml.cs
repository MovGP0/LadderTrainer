using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
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
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Take(1)
                .Subscribe(vm =>
                {
                    this.OneWayBind(vm!,
                            x => x.TargetRepetitions,
                            v  => v.TargetLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.CompletedRepetitions,
                            v  => v.CompletedLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.OverallTimeText,
                            v  => v.TotalTimeLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.IsStatisticsVisible,
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
