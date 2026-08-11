using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;
    [SerializeField] CinemachineCamera betsCamera;

    private RoundStateManager roundStateManager;
    private WinningSequence winningSequence;

    [Inject]
    public void Construct(RoundStateManager roundStateManager, WinningSequence winningSequence)
    {
        this.roundStateManager = roundStateManager;
        this.winningSequence = winningSequence;

    }

    public void Start()
    {
        roundStateManager.StateChanged += OnStateChanged;
        winningSequence.CountMoney += OnCountMoney;
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        switch (state) 
        {
            case RoundStateManager.RoundState.NotActive:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                break;
            case RoundStateManager.RoundState.WheelSpining:
            case RoundStateManager.RoundState.ChoosingReward:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                break;
            case RoundStateManager.RoundState.BetResults:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(true);
                break;
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                break;
        }
    }
    private void OnCountMoney(int bet, float multiplier, int reward)
    {
        betsCamera.gameObject.SetActive(false);
        roundCamera.gameObject.SetActive(true);
    }
}
