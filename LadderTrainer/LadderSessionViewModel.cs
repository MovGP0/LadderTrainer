using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LadderTrainer;

public partial class LadderSessionViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    private readonly int[] _ladderPattern = [1, 2, 3, 4, 5, 4, 3, 2, 1];
    private int _currentPatternIndex;

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
    [ObservableAsProperty] private bool _isSettingsEnabled = true;

    private readonly IObservable<bool> _canStart;
    private readonly CompositeDisposable _derivedPropertySubscriptions = new();

    // Timing
    private readonly Stopwatch _overallStopwatch = new();
    private readonly Stopwatch _phaseStopwatch = new();
    private TimeSpan _lastSetDuration = TimeSpan.Zero;

    public LadderSessionViewModel()
    {
        // CanExecute for StartCommand – only valid for 1..100 reps
        _canStart = this.WhenAnyValue(vm => vm.TargetRepetitions,
            reps => reps is >= 1 and <= 100);

        // Derived properties (OAPH) – not tied to activation lifecycle
        _remainingRepetitionsHelper = this.WhenAnyValue(vm => vm.TargetRepetitions, vm => vm.CompletedRepetitions,
                (target, completed) => Math.Max(0, target - completed))
            .ToProperty(this, vm => vm.RemainingRepetitions, initialValue: _targetRepetitions, scheduler: RxApp.MainThreadScheduler);
        _derivedPropertySubscriptions.Add(_remainingRepetitionsHelper);

        var phaseStream = this.WhenAnyValue(vm => vm.Phase)
            .StartWith(Phase);

        _isWorkoutActiveHelper = phaseStream
            .Select(p => p == SessionPhase.Workout)
            .ToProperty(this, vm => vm.IsWorkoutActive, initialValue: false, scheduler: RxApp.MainThreadScheduler);
        _derivedPropertySubscriptions.Add(_isWorkoutActiveHelper);

        _isRestActiveHelper = phaseStream
            .Select(p => p == SessionPhase.Rest)
            .ToProperty(this, vm => vm.IsRestActive, initialValue: false, scheduler: RxApp.MainThreadScheduler);
        _derivedPropertySubscriptions.Add(_isRestActiveHelper);

        _isStatisticsVisibleHelper = phaseStream
            .Select(p => p == SessionPhase.Completed)
            .ToProperty(this, vm => vm.IsStatisticsVisible, initialValue: false, scheduler: RxApp.MainThreadScheduler);
        _derivedPropertySubscriptions.Add(_isStatisticsVisibleHelper);

        _isSettingsEnabledHelper = phaseStream
            .Select(p => p is SessionPhase.Idle or SessionPhase.Completed)
            .ToProperty(this, vm => vm.IsSettingsEnabled, initialValue: true, scheduler: RxApp.MainThreadScheduler);
        _derivedPropertySubscriptions.Add(_isSettingsEnabledHelper);

        // Activation: timers
        this.WhenActivated(disposables =>
        {
            // UI tick / rest logic
            Observable.Interval(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(_ => Tick())
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
        _currentPatternIndex = 0;
        CurrentSetRepetitions = 0;
        _lastSetDuration = TimeSpan.Zero;

        _overallStopwatch.Reset();
        _phaseStopwatch.Reset();

        OverallTimeText = "00:00";
        CurrentPhaseTimeText = "00:00";
        CurrentPhaseLabel = "Workout";

        Phase = SessionPhase.Idle;

        _overallStopwatch.Start();
        AdvanceToNextSet();
    }

    // ReactiveCommand: SetDoneCommand
    [ReactiveCommand]
    private void SetDone()
    {
        // Only meaningful during workout; views already hide this button otherwise.
        if (Phase != SessionPhase.Workout)
            return;

        _phaseStopwatch.Stop();
        _lastSetDuration = _phaseStopwatch.Elapsed;

        CompletedRepetitions += CurrentSetRepetitions;

        if (CompletedRepetitions >= TargetRepetitions)
        {
            Phase = SessionPhase.Completed;
            _overallStopwatch.Stop();
            CurrentPhaseLabel = "Completed";
            UpdateTimes();
            return;
        }

        // Rest phase – rest duration equals length of the last set
        Phase = SessionPhase.Rest;
        CurrentPhaseLabel = "Rest";
        _phaseStopwatch.Restart();
        UpdateTimes();
    }

    private void AdvanceToNextSet()
    {
        if (CompletedRepetitions >= TargetRepetitions)
        {
            Phase = SessionPhase.Completed;
            _overallStopwatch.Stop();
            _phaseStopwatch.Reset();
            CurrentPhaseLabel = "Completed";
            UpdateTimes();
            return;
        }

        CurrentSetRepetitions = _ladderPattern[_currentPatternIndex];

        _currentPatternIndex++;
        if (_currentPatternIndex >= _ladderPattern.Length)
            _currentPatternIndex = 0;

        Phase = SessionPhase.Workout;
        CurrentPhaseLabel = $"Workout: {CurrentSetRepetitions} reps";

        _phaseStopwatch.Restart();
        UpdateTimes();
    }

    private void Tick()
    {
        if (Phase is SessionPhase.Idle)
            return;

        UpdateTimes();

        if (Phase == SessionPhase.Rest &&
            _lastSetDuration > TimeSpan.Zero &&
            _phaseStopwatch.Elapsed >= _lastSetDuration)
        {
            // Rest finished, move to next ladder step
            AdvanceToNextSet();
        }
    }

    private void UpdateTimes()
    {
        OverallTimeText = FormatTime(_overallStopwatch.Elapsed);

        switch (Phase)
        {
            case SessionPhase.Workout:
                CurrentPhaseTimeText = $"Set time: {FormatTime(_phaseStopwatch.Elapsed)}";
                break;

            case SessionPhase.Rest:
                var remaining = _lastSetDuration - _phaseStopwatch.Elapsed;
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
