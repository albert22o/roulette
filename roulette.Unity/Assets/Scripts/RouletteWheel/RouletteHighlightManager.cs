using UnityEngine;
using Zenject;

public class RouletteHighlightManager : MonoBehaviour
{
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private RouletteBallsManager ballsManager;
    [SerializeField] private RoundStateManager roundStateManager; 

    private BallResults ballResults;

    [Inject]
    public void Construct(BallResults ballResults, RoundStateManager roundStateManager)
    {
        this.ballResults = ballResults;
        this.roundStateManager = roundStateManager; 
    }

    private void OnEnable()
    {
        if (ballsManager != null)
        {
            ballsManager.AllBallsStopped += OnAllBallsStopped;
            ballsManager.SimulationStarted += OnSimulationStarted;
        }

        if (ballResults != null)
        {
            ballResults.BallSelected += OnBallSelected;
        }

        if (roundStateManager != null)
        {
            roundStateManager.StateChanged += OnRoundStateChanged;
        }
    }

    private void OnDisable()
    {
        if (ballsManager != null)
        {
            ballsManager.AllBallsStopped -= OnAllBallsStopped;
            ballsManager.SimulationStarted -= OnSimulationStarted;
        }

        if (ballResults != null)
        {
            ballResults.BallSelected -= OnBallSelected;
        }

        if (roundStateManager != null)
        {
            roundStateManager.StateChanged -= OnRoundStateChanged;
        }
    }

    private void OnRoundStateChanged(RoundStateManager.RoundState newState)
    {
        if (newState != RoundStateManager.RoundState.ChoosingReward)
        {
            ResetAllHighlights();
        }
    }

    private void OnSimulationStarted()
    {
        ResetAllHighlights();
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

    private void HighlightField(int fieldNumber)
    {
        int positionOnWheel = System.Array.IndexOf(rouletteWheel.FieldsOrder, fieldNumber);

        if (positionOnWheel >= 0 && positionOnWheel < rouletteWheel.Fields.Length)
        {
            rouletteWheel.Fields[positionOnWheel].IsHighlighted = true;
        }
    }

    private void ResetAllHighlights()
    {
        for (int i = 0; i < rouletteWheel.Fields.Length; i++)
        {
            rouletteWheel.Fields[i].IsHighlighted = false;
        }
    }
}