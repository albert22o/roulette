using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class PlaquesSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sectionText;
    private PlaqueHolder plaqueHolder;

    [Inject]
    public void Construct(PlaqueHolder plaqueHolder)
    {
        this.plaqueHolder = plaqueHolder;
    }

    public void UpdateData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>ПЛАКИ</b>");

        int count = plaqueHolder.plaques?.Count ?? 0;
        sb.AppendLine($"Всего: {count}");

        if (plaqueHolder.plaques != null)
        {
            foreach (var plaque in plaqueHolder.plaques)
                sb.AppendLine($"• {plaque.GetType().Name}");
        }
        sectionText.text = sb.ToString();
    }
}