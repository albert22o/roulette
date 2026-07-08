using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class FoodSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sectionText;
    private FoodHolder foodHolder;

    [Inject]
    public void Construct(FoodHolder foodHolder)
    {
        this.foodHolder = foodHolder;
    }

    public void UpdateData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>ЕДА</b>");

        int count = foodHolder.AppliedHistory?.Count ?? 0;
        sb.AppendLine($"Применено: {count}");

        if (foodHolder.AppliedHistory != null)
        {
            foreach (var food in foodHolder.AppliedHistory)
                sb.AppendLine($"• {food.GetType().Name}");
        }
        sectionText.text = sb.ToString();
    }
}