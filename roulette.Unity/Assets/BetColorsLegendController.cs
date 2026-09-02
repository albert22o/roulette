using UnityEngine;
using Zenject;

public class BetColorsLegendController : MonoBehaviour
{
    private RoundStateManager roundStateManager;

    [Inject]
    private void Construct(RoundStateManager roundStateManager)
    {
        this.roundStateManager = roundStateManager;
    }

    void Start()
    {
        roundStateManager.StateChanged += OnRoundStateChanged;
        gameObject.SetActive(false);
    }

    private void OnRoundStateChanged(RoundStateManager.RoundState state)
    {
        gameObject.SetActive(state == RoundStateManager.RoundState.WheelSpining ||
            state == RoundStateManager.RoundState.ChoosingReward);
    }
}
