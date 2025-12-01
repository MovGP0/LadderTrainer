using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Maui;

namespace LadderTrainer.Views;

public partial class PauseView : ReactiveContentView<LadderSessionViewModel>
{
    public PauseView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Reuse CurrentPhaseTimeText (during rest it is "Rest remaining: mm:ss")
            this.OneWayBind(ViewModel,
                    vm => vm.CurrentPhaseTimeText,
                    v  => v.RestTimeLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.OverallTimeText,
                    v  => v.OverallTimeLabel.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.IsRestActive,
                    v  => v.IsVisible)
                .DisposeWith(disposables);
        });
    }
}