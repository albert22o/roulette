using TMPro;
using UnityEngine;
using Zenject;

public class QuotasSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentQuotaText;
    [SerializeField] private TextMeshProUGUI completedQuotasText;     

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
        if (completedQuotasText != null)
            completedQuotasText.text = $"Пройдено квот: {progressionManager.CurrentIndex}";

        if (currentQuotaText != null)
            currentQuotaText.text = $"Текущая квота: {roundManager.Quote}/{roundManager.Goal}";
    }
}