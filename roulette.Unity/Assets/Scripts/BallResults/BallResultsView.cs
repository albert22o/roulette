using TMPro;
using UnityEngine;
using Zenject;

public class BallResultsView : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private BallResults ballResults;

    [Inject]
    public void Construct(BallResults ballResults)
    {
        this.ballResults = ballResults;

        ballResults.BallHovered += ShowInfo;
        ballResults.BallUnHovered += HideInfo;

        HideInfo();
    }

    private void ShowInfo(BallInfoData ballInfoData)
    {
        infoPanel.SetActive(true);
        nameText.text = ballInfoData.Name;
        descriptionText.text = ballInfoData.Description;
    }

    private void HideInfo()
    {
        infoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (ballResults != null)
        {
            ballResults.BallHovered -= ShowInfo;
            ballResults.BallUnHovered -= HideInfo;
        }
    }
}
