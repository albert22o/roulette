using UnityEngine;

public class RerollCostViewConnector : MonoBehaviour
{
    [SerializeField] private ShopRerollButton rerollButton;
    [SerializeField] private CostView costView;

    private void OnEnable()
    {
        rerollButton.CostChanged += OnCostChanged;
    }

    private void OnDisable()
    {
        rerollButton.CostChanged -= OnCostChanged;
    }

    private void OnCostChanged(int cost)
    {
        costView.SetCost(cost);
    }
}
