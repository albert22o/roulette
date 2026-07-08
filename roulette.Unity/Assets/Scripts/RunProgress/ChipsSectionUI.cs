using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class ChipsSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sectionText;
    private ChipHolder chipHolder;

    [Inject]
    public void Construct(ChipHolder chipHolder)
    {
        this.chipHolder = chipHolder;
    }

    public void UpdateData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>ФИШКИ</b>");

        int count = 0;
        foreach (var chip in chipHolder.Chips)
        {
            if (chip != null)
            {
                count++;
                sb.AppendLine($"• {chip.ChipData.Name}");
            }
        }
        sb.AppendLine($"Всего: {count}/{chipHolder.Capacity}");
        sectionText.text = sb.ToString();
    }
}