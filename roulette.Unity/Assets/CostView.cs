using TMPro;
using UnityEngine;

public class CostView : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmPro;

    public void SetCost(int cost)
    {
        tmPro.text = cost.ToString();
    }
}
