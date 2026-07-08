using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class BallsSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sectionText;
    private BallHolder ballHolder;

    [Inject]
    public void Construct(BallHolder ballHolder)
    {
        this.ballHolder = ballHolder;
    }

    public void UpdateData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>ШАРИКИ</b>");

        int count = ballHolder.Balls?.Count ?? 0;
        sb.AppendLine($"Всего: {count}");

        if (ballHolder.Balls != null)
        {
            foreach (var ball in ballHolder.Balls)
                sb.AppendLine($"• {ball.BallInfoData.Name}");
        }
        sectionText.text = sb.ToString();
    }
}