using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialTableHighlighter : MonoBehaviour
{
    [SerializeField] private BetTableView _table;
    [SerializeField] private float _blinkInterval = 0.5f;

    private readonly Dictionary<BetButton, BetButton.HighlightPhase> _buttonTargets = new();
    private readonly List<BottomSectionZone> _zoneTargets = new();
    private Coroutine _blinkRoutine;

    private TutorialTableHighlight _commands;
    private TableInfo _tableInfo;

    [Inject]
    public void Construct(TutorialTableHighlight commands, TableInfo tableInfo)
    {
        _commands = commands;
        _tableInfo = tableInfo;

        _commands.BlinkAllNumbersRequested += BlinkAllNumbers;
        _commands.BlinkBetRequested += OnBlinkBet;
        _commands.StopRequested += Stop;
    }

    private void OnDestroy()
    {
        if (_commands == null) return;
        _commands.BlinkAllNumbersRequested -= BlinkAllNumbers;
        _commands.BlinkBetRequested -= OnBlinkBet;
        _commands.StopRequested -= Stop;
    }

    private void OnBlinkBet(int index) => BlinkBet(_tableInfo.PocketMapping[index]);

    /// <summary>Шаг 1: мигают все числа.</summary>
    private void BlinkAllNumbers()
    {
        Stop();
        foreach (var b in _table.Buttons)
            _buttonTargets[b] = BetButton.HighlightPhase.Number;
        StartBlink();
    }

    /// <summary>Шаг 3: цвет, дюжина и число ставки мигают разными материалами.</summary>
    private void BlinkBet(BetInfo bet)
    {
        Stop();
        foreach (var b in _table.Buttons)
        {
            // Приоритет: число > дюжина > цвет
            if (b.FieldNum == bet.FieldNumber)
                _buttonTargets[b] = BetButton.HighlightPhase.Number;
            else if (b.Dozen == bet.Dozen)
                _buttonTargets[b] = BetButton.HighlightPhase.Dozen;
            else if (b.Color == bet.FieldColor)
                _buttonTargets[b] = BetButton.HighlightPhase.Color;
        }
        foreach (var z in _table.ColorZones)
            if (z.Color == bet.FieldColor) _zoneTargets.Add(z);
        foreach (var z in _table.DozenZones)
            if (z.Dozen == bet.Dozen) _zoneTargets.Add(z);

        StartBlink();
    }

    private void Stop()
    {
        if (_blinkRoutine != null)
        {
            StopCoroutine(_blinkRoutine);
            _blinkRoutine = null;
        }
        Apply(false);
        _buttonTargets.Clear();
        _zoneTargets.Clear();
    }

    private void StartBlink() => _blinkRoutine = StartCoroutine(BlinkRoutine());

    private IEnumerator BlinkRoutine()
    {
        bool on = true;
        while (true)
        {
            Apply(on);
            yield return new WaitForSeconds(_blinkInterval);
            on = !on;
        }
    }

    private void Apply(bool on)
    {
        foreach (var kv in _buttonTargets)
            kv.Key.Highlight(on, on ? kv.Value : BetButton.HighlightPhase.None);
        foreach (var z in _zoneTargets)
            z.Highlight(on);
    }
}