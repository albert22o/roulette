using UnityEngine;
using Zenject;

public class MoneyView : MonoBehaviour
{
    [SerializeField] private DrumNumberDisplay display;

    private BetTable betTable;

    [Inject]
    public void Construct(BetTable betTable)
    {
        this.betTable = betTable;
    }

    private void OnEnable()
    {
        betTable.ChangeAvailableSum += UpdateDisplay;
        UpdateDisplay(betTable.AvailableSum);
    }

    private void OnDisable()
    {
        betTable.ChangeAvailableSum -= UpdateDisplay;
    }

    private void UpdateDisplay(int amount)
    {
        display.SetNumber(amount);
    }
}

