using System;
using UnityEngine;
using Zenject;

public class RouletteHighlightManager : MonoBehaviour
{
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private RouletteBallsManager ballsManager;

    private RoundStateManager roundStateManager;
    private BallResults ballResults;

    [Inject]
    public void Construct(BallResults ballResults, RoundStateManager roundStateManager, IRouletteWheel rouletteWheel)
    {
        this.ballResults = ballResults;
        this.roundStateManager = roundStateManager;
    }

    private void OnEnable()
    {
        ballsManager.AllBallsStopped += OnAllBallsStopped;
        ballResults.BallSelected += OnBallSelected;
        roundStateManager.StateChanged += OnRoundStateChanged;
        rouletteWheel.Rerolled += OnReroll;
    }


    private void OnDisable()
    {
        ballsManager.AllBallsStopped -= OnAllBallsStopped;
        ballResults.BallSelected -= OnBallSelected;
        roundStateManager.StateChanged -= OnRoundStateChanged;
        rouletteWheel.Rerolled -= OnReroll;
    }

    private void OnRoundStateChanged(RoundStateManager.RoundState newState)
    {
        if (newState == RoundStateManager.RoundState.WheelSpining ||
            newState == RoundStateManager.RoundState.PlacingBets ||
            newState == RoundStateManager.RoundState.Start ||
            newState == RoundStateManager.RoundState.NotActive ||
            newState == RoundStateManager.RoundState.BetResults)
        {
            ResetAllHighlights();
        }
    }

    private void OnBallSelected(HitInfo hitInfo)
    {
        ResetAllHighlights();

        if (hitInfo != null)
        {
            HighlightField(hitInfo.FieldIndex);
        }
    }

    private void OnAllBallsStopped(HitInfo[] hitsInfo)
    {
        ResetAllHighlights();

        foreach (var hit in hitsInfo)
        {
            HighlightField(hit.FieldIndex);
        }
    }

    private void HighlightField(int fieldIndex)
    {
        int positionOnWheel = System.Array.IndexOf(rouletteWheel.FieldsOrder, fieldIndex);

        if (positionOnWheel >= 0 && positionOnWheel < rouletteWheel.HighlightStates.Length)
        {
            rouletteWheel.HighlightStates[positionOnWheel].IsRimHighlighted = true;
        }
    }

    private void OnReroll()
    {
        ResetAllHighlights();
    }

    private void ResetAllHighlights()
    {
        for (int i = 0; i < rouletteWheel.HighlightStates.Length; i++)
        {
            rouletteWheel.HighlightStates[i].IsRimHighlighted = false;
        }
    }
}