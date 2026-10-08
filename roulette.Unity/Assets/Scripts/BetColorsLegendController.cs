using TMPro;
using UnityEngine;
using Zenject;

public class BetColorsLegendController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText, dozenText, colorText;
    private RoundStateManager roundStateManager;
    private BetCalculator betCalculator;

    [Inject]
    private void Construct(RoundStateManager roundStateManager, BetCalculator betCalculator)
    {
        this.roundStateManager = roundStateManager;
        this.betCalculator = betCalculator;
    }

    void Start()
    {
        roundStateManager.StateChanged += OnRoundStateChanged;
        betCalculator.ColorMultiplierChanged += OnColorMultiplierChanged;
        betCalculator.DozenMultiplierChanged += OnDozenMultiplierChanged;
        betCalculator.NumMultiplierChanged += OnNumMultiplierChanged;
        OnNumMultiplierChanged(betCalculator.NumMultiplier);
        OnDozenMultiplierChanged(betCalculator.DozenMultiplier);
        OnColorMultiplierChanged(betCalculator.ColorMultiplier);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        roundStateManager.StateChanged -= OnRoundStateChanged;
        betCalculator.ColorMultiplierChanged -= OnColorMultiplierChanged;
        betCalculator.DozenMultiplierChanged -= OnDozenMultiplierChanged;
        betCalculator.NumMultiplierChanged -= OnNumMultiplierChanged;
    }

    private void OnNumMultiplierChanged(int obj)
    {
        numText.text = $"×{obj}";
    }

    private void OnDozenMultiplierChanged(int obj)
    {
        dozenText.text = $"×{obj}";
    }

    private void OnColorMultiplierChanged(int obj)
    {
        colorText.text = $"×{obj}";
    }


    private void OnRoundStateChanged(RoundStateManager.RoundState state)
    {
        gameObject.SetActive(state == RoundStateManager.RoundState.WheelSpining ||
            state == RoundStateManager.RoundState.ChoosingReward);
    }
}
