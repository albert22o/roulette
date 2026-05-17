using UnityEngine;

public class BallsVendingMachineCostViewConnector : MonoBehaviour
{
    [SerializeField] private BallsVendingMachine vendingMachine;
    [SerializeField] private CostView costView;

    private void OnEnable()
    {
        vendingMachine.CostChanged += OnCostChanged;
    }

    private void OnDisable()
    {
        vendingMachine.CostChanged -= OnCostChanged;
    }

    private void OnCostChanged(int cost)
    {
        costView.SetCost(cost);
    }
}
