using System;
using Zenject;
using static GameStateManager;

public class TutorialSignalBridge : IInitializable, IDisposable
{
    private readonly TutorialSignals _signals;
    private readonly RoundManager _round;
    private readonly RouletteWheel _wheel;
    private readonly GameStateManager _gameState;
    private readonly ProgressionManager _progression;
    private readonly BossReward _bossReward;

    private bool _betFinished;
    private bool _bossHintRaised;
    private bool _bossChipsPending;

    public TutorialSignalBridge(TutorialSignals signals, RoundManager round, RouletteWheel wheel,
        GameStateManager gameState, ProgressionManager progression, BossReward bossReward)
    {
        _signals = signals;
        _round = round;
        _wheel = wheel;
        _gameState = gameState;
        _progression = progression;
        _bossReward = bossReward;
    }

    public void Initialize()
    {
        _round.Spun += OnSpun;
        _round.AbleToWinChanged += OnAbleToWinChanged;
        _round.Win += OnWin;
        _wheel.AllBallsStopped += OnAllBallsStopped;
        _gameState.GameStateChanged += OnGameStateChanged;
        _bossReward.BossChipsGiven += OnBossChipsGiven;

        if (_gameState.CurrentState == GameState.BettingCycle)
            _signals.Raise(TutorialSignal.TableEntered);
    }

    public void Dispose()
    {
        _round.Spun -= OnSpun;
        _round.AbleToWinChanged -= OnAbleToWinChanged;
        _round.Win -= OnWin;
        _wheel.AllBallsStopped -= OnAllBallsStopped;
        _gameState.GameStateChanged -= OnGameStateChanged;
        _bossReward.BossChipsGiven -= OnBossChipsGiven;
    }

    private void OnSpun() => _signals.Raise(TutorialSignal.SpinStarted);
    private void OnAllBallsStopped(HitInfo[] _) => _signals.Raise(TutorialSignal.SpinFinished);

    private void OnAbleToWinChanged(bool able)
    {
        if (able) _signals.Raise(TutorialSignal.QuotaCompleted);
    }

    private void OnWin()
    {
        _betFinished = true;
        _signals.Raise(TutorialSignal.BetFinished);
    }

    // Босс-фишку могли выдать до или после выхода из экрана наград
    private void OnBossChipsGiven()
    {
        if (_gameState.CurrentState == GameState.FreeRoam)
            _signals.Raise(TutorialSignal.ThirdQuotaRewardsChosen);
        else
            _bossChipsPending = true;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.BettingCycle)
        {
            _signals.Raise(TutorialSignal.TableEntered);

            // Подсказка только на первой босс-квоте
            if (!_bossHintRaised && _progression.Current.IsBoss)
            {
                _bossHintRaised = true;
                _signals.Raise(TutorialSignal.ThirdQuotaStarted);
            }
        }

        if (state == GameState.FreeRoam)
        {
            if (_betFinished)
            {
                _betFinished = false;
                _signals.Raise(TutorialSignal.LeftTable);
            }

            if (_bossChipsPending)
            {
                _bossChipsPending = false;
                _signals.Raise(TutorialSignal.ThirdQuotaRewardsChosen);
            }
        }

        if (state == GameState.Shopping)
            _signals.Raise(TutorialSignal.ShopEntered);

        if (state == GameState.PlaqueShopping)
            _signals.Raise(TutorialSignal.AltarEntered);
    }
}