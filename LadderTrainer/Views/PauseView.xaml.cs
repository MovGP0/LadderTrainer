using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
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
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Take(1)
                .Subscribe(vm =>
                {
                    // Reuse CurrentPhaseTimeText (during rest it is "Rest remaining: mm:ss")
                    this.OneWayBind(vm!,
                            x => x.CurrentPhaseTimeText,
                            v  => v.RestTimeLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.OverallTimeText,
                            v  => v.OverallTimeLabel.Text)
                        .DisposeWith(disposables);

                    this.OneWayBind(vm!,
                            x => x.IsRestActive,
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
