using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class QuotasSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sectionText;
    private ProgressionManager progressionManager;
    private RoundManager roundManager;

    [Inject]
    public void Construct(ProgressionManager progressionManager, RoundManager roundManager)
    {
        this.progressionManager = progressionManager;
        this.roundManager = roundManager;
    }

    public void UpdateData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>КВОТЫ</b>");
        sb.AppendLine($"Пройдено: {progressionManager.CurrentIndex}");
        sb.AppendLine($"Текущая: {roundManager.Quote}/{roundManager.Goal}");
        sectionText.text = sb.ToString();
    }
}