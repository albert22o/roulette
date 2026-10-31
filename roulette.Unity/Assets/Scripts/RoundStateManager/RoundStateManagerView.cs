using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;
    [SerializeField] CinemachineCamera betsCamera;
    CancelInputHandler cancelInputHandler;

    private RoundStateManager roundStateManager;
    private WinningSequence winningSequence;

    [Inject]
    public void Construct(RoundStateManager roundStateManager, WinningSequence winningSequence, CancelInputHandler cancelInputHandler)
    {
        this.roundStateManager = roundStateManager;
        this.winningSequence = winningSequence;
        this.cancelInputHandler = cancelInputHandler;
    }

    public void Start()
    {
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        switch (state) 
        {
            case RoundStateManager.RoundState.NotActive:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                cancelInputHandler.Toggle(false);
                break;
            case RoundStateManager.RoundState.WheelSpining:
            case RoundStateManager.RoundState.ChoosingReward:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                cancelInputHandler.Toggle(false);
                break;
            case RoundStateManager.RoundState.BetResults:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(true);
                cancelInputHandler.Toggle(false);
                break;
            case RoundStateManager.RoundState.CountMoney:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                cancelInputHandler.Toggle(false);
                break;
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                cancelInputHandler.Toggle(true);
                break;
        }
    }
}
