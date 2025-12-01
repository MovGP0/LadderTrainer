using System.Diagnostics;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LadderTrainer;

public partial class LadderSessionViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    private readonly int[] ladderPattern = [1, 2, 3, 4, 5, 4, 3, 2, 1];
    private int currentPatternIndex;

    [Reactive] private int _targetRepetitions = 50;
    [Reactive] private int _completedRepetitions;
    [Reactive] private int _currentSetRepetitions;
    [Reactive] private SessionPhase _phase = SessionPhase.Idle;

    [Reactive] private string _overallTimeText = "00:00";
    [Reactive] private string _currentPhaseTimeText = "00:00";
    [Reactive] private string _currentPhaseLabel = "Idle";

    [ObservableAsProperty] private int _remainingRepetitions;
    [ObservableAsProperty] private bool _isWorkoutActive;
    [ObservableAsProperty] private bool _isRestActive;
    [ObservableAsProperty] private bool _isStatisticsVisible;
    [ObservableAsProperty] private bool _isSettingsEnabled;

    private readonly IObservable<bool> _canStart;

    // Timing
    private readonly Stopwatch overallStopwatch = new();
    private readonly Stopwatch phaseStopwatch = new();
    private TimeSpan lastSetDuration = TimeSpan.Zero;

    public LadderSessionViewModel()
    {
        // CanExecute for StartCommand – only valid for 1..100 reps
        _canStart = this.WhenAnyValue(vm => vm.TargetRepetitions,
            reps => reps is >= 1 and <= 100);

        // Activation: timers + derived properties
        this.WhenActivated(disposables =>
        {
            // UI tick / rest logic
            Observable.Interval(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(_ => Tick())
                .DisposeWith(disposables);

            // Remaining repetitions derived from Target + Completed
            this.WhenAnyValue(vm => vm.TargetRepetitions, vm => vm.CompletedRepetitions,
                    (target, completed) => Math.Max(0, target - completed))
                .ToProperty(this, vm => vm.RemainingRepetitions)
                .DisposeWith(disposables);

            // Phase-dependent boolean flags (workout / rest / statistics / settings)
            var phaseChanges = this.WhenAnyValue(vm => vm.Phase).Publish().RefCount();

            phaseChanges
                .Select(p => p == SessionPhase.Workout)
                .ToProperty(this, vm => vm.IsWorkoutActive)
                .DisposeWith(disposables);

            phaseChanges
                .Select(p => p == SessionPhase.Rest)
                .ToProperty(this, vm => vm.IsRestActive)
                .DisposeWith(disposables);

            phaseChanges
                .Select(p => p == SessionPhase.Completed)
                .ToProperty(this, vm => vm.IsStatisticsVisible)
                .DisposeWith(disposables);

            phaseChanges
                .Select(p => p is SessionPhase.Idle or SessionPhase.Completed)
                .ToProperty(this, vm => vm.IsSettingsEnabled)
                .DisposeWith(disposables);
        });
    }

    // ReactiveCommand: StartCommand
    // CanExecute is wired to _canStart via attribute.
    [ReactiveCommand(CanExecute = nameof(_canStart))]
    private void Start()
    {
        // Reset session
        CompletedRepetitions = 0;
        currentPatternIndex = 0;
        CurrentSetRepetitions = 0;
        lastSetDuration = TimeSpan.Zero;

        overallStopwatch.Reset();
        phaseStopwatch.Reset();

        OverallTimeText = "00:00";
        CurrentPhaseTimeText = "00:00";
        CurrentPhaseLabel = "Workout";

        Phase = SessionPhase.Idle;

        overallStopwatch.Start();
        AdvanceToNextSet();
    }

    // ReactiveCommand: SetDoneCommand
    [ReactiveCommand]
    private void SetDone()
    {
        // Only meaningful during workout; views already hide this button otherwise.
        if (Phase != SessionPhase.Workout)
            return;

        phaseStopwatch.Stop();
        lastSetDuration = phaseStopwatch.Elapsed;

        CompletedRepetitions += CurrentSetRepetitions;

        if (CompletedRepetitions >= TargetRepetitions)
        {
            Phase = SessionPhase.Completed;
            overallStopwatch.Stop();
            CurrentPhaseLabel = "Completed";
            UpdateTimes();
            return;
        }

        // Rest phase – rest duration equals length of the last set
        Phase = SessionPhase.Rest;
        CurrentPhaseLabel = "Rest";
        phaseStopwatch.Restart();
        UpdateTimes();
    }

    private void AdvanceToNextSet()
    {
        if (CompletedRepetitions >= TargetRepetitions)
        {
            Phase = SessionPhase.Completed;
            overallStopwatch.Stop();
            phaseStopwatch.Reset();
            CurrentPhaseLabel = "Completed";
            UpdateTimes();
            return;
        }

        CurrentSetRepetitions = ladderPattern[currentPatternIndex];

        currentPatternIndex++;
        if (currentPatternIndex >= ladderPattern.Length)
            currentPatternIndex = 0;

        Phase = SessionPhase.Workout;
        CurrentPhaseLabel = $"Workout: {CurrentSetRepetitions} reps";

        phaseStopwatch.Restart();
        UpdateTimes();
    }

    private void Tick()
    {
        if (Phase is SessionPhase.Idle)
            return;

        UpdateTimes();

        if (Phase == SessionPhase.Rest &&
            lastSetDuration > TimeSpan.Zero &&
            phaseStopwatch.Elapsed >= lastSetDuration)
        {
            // Rest finished, move to next ladder step
            AdvanceToNextSet();
        }
    }

    private void UpdateTimes()
    {
        OverallTimeText = FormatTime(overallStopwatch.Elapsed);

        switch (Phase)
        {
            case SessionPhase.Workout:
                CurrentPhaseTimeText = $"Set time: {FormatTime(phaseStopwatch.Elapsed)}";
                break;

            case SessionPhase.Rest:
                var remaining = lastSetDuration - phaseStopwatch.Elapsed;
                if (remaining < TimeSpan.Zero)
                    remaining = TimeSpan.Zero;
                CurrentPhaseTimeText = $"Rest remaining: {FormatTime(remaining)}";
                break;

            case SessionPhase.Completed:
                CurrentPhaseTimeText = "Workout completed";
                break;

            default:
                CurrentPhaseTimeText = "00:00";
                break;
        }
    }

    private static string FormatTime(TimeSpan t)
        => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
}