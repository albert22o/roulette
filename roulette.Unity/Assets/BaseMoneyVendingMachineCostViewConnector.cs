using UnityEngine;

public class BaseMoneyVendingMachineCostViewConnector : MonoBehaviour
{
    [SerializeField] private BaseMoneyVendingMachine vendingMachine;
    [SerializeField] private CostView costView;

    private void OnEnable()
    {
        vendingMachine.CostChanged += OnCostChanged;
        costView.SetCost(vendingMachine.Cost);
    }

    private void OnDisable()
    {
        vendingMachine.CostChanged -= OnCostChanged;
    }

    private void OnCostChanged(int newCost)
    {
        costView.SetCost(newCost);
    }
}
