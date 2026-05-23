using UnityEngine;

public class BaseMoneyVendingMachineCostViewConnector : MonoBehaviour
{
    [SerializeField] private BaseMoneyVendingMachine vendingMachine;
    [SerializeField] private DrumNumberDisplay costView;

    private void OnEnable()
    {
        vendingMachine.CostChanged += OnCostChanged;
        costView.SetNumber(vendingMachine.Cost);
    }

    private void OnDisable()
    {
        vendingMachine.CostChanged -= OnCostChanged;
    }

    private void OnCostChanged(int newCost)
    {
        costView.SetNumber(newCost);
    }
}
