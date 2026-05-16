using UnityEngine;
using Zenject;

public class BallResultView : MonoBehaviour
{
    [SerializeField] TableButton confirmButton;

    private BallResults ballResults;

    [Inject]
    public void Consturct(BallResults ballResults)
    {
        this.ballResults = ballResults;
    }

    public void Init()
    {
        confirmButton.Clicked += OnConfirmButtonClicked;
    }

    private void OnConfirmButtonClicked()
    {
        ballResults.SelectBall();
    }
}