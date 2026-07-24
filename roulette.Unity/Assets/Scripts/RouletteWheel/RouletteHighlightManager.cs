using UnityEngine;
using Zenject;

public class RouletteHighlightManager : MonoBehaviour
{
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private RouletteBallsManager ballsManager;

    private BallResults ballResults;

    [Inject]
    public void Construct(BallResults ballResults)
    {
        this.ballResults = ballResults;
        Debug.Log($"[RouletteHighlightManager] BallResults injected: {ballResults != null}");
    }

    private void OnEnable()
    {
        Debug.Log("[RouletteHighlightManager] OnEnable called");

        if (ballsManager != null)
        {
            ballsManager.AllBallsStopped += OnAllBallsStopped;
            ballsManager.SimulationStarted += OnSimulationStarted;
            Debug.Log("[RouletteHighlightManager] Subscribed to ballsManager events");
        }
        else
        {
            Debug.LogWarning("[RouletteHighlightManager] ballsManager is NULL!");
        }

        if (ballResults != null)
        {
            ballResults.BallSelected += OnBallSelected;
            Debug.Log("[RouletteHighlightManager] Subscribed to BallSelected event");
        }
        else
        {
            Debug.LogWarning("[RouletteHighlightManager] ballResults is NULL! Zenject injection failed.");
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
    }

    private void OnSimulationStarted()
    {
        Debug.Log("[RouletteHighlightManager] SimulationStarted - resetting highlights");
        ResetAllHighlights();
    }

    private void OnBallSelected(HitInfo hitInfo)
    {
        Debug.Log($"[RouletteHighlightManager] BallSelected called! hitInfo is {(hitInfo == null ? "null" : "valid")}");
        ResetAllHighlights();

        if (hitInfo != null)
        {
            Debug.Log($"[RouletteHighlightManager] Highlighting field index: {hitInfo.FieldIndex}");
            HighlightField(hitInfo.FieldIndex);
        }
    }

    private void OnAllBallsStopped(HitInfo[] hitsInfo)
    {
        Debug.Log($"[RouletteHighlightManager] AllBallsStopped - {hitsInfo.Length} balls stopped");
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
            Debug.Log($"[RouletteHighlightManager] Highlighting field {fieldNumber} at position {positionOnWheel}");
            rouletteWheel.Fields[positionOnWheel].IsHighlighted = true;
        }
    }

    private void ResetAllHighlights()
    {
        Debug.Log("[RouletteHighlightManager] Resetting all highlights");
        for (int i = 0; i < rouletteWheel.Fields.Length; i++)
        {
            rouletteWheel.Fields[i].IsHighlighted = false;
        }
    }
}