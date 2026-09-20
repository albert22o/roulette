using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;
    [SerializeField] CinemachineCamera betsCamera;
    [SerializeField] CancelInputHandler inputHandler;

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
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        Debug.Log(state);
        switch (state) 
        {
            case RoundStateManager.RoundState.NotActive:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                inputHandler.enabled = false;
                break;
            case RoundStateManager.RoundState.WheelSpining:
            case RoundStateManager.RoundState.ChoosingReward:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                inputHandler.enabled = false;
                break;
            case RoundStateManager.RoundState.BetResults:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(true);
                inputHandler.enabled = false;
                break;
            case RoundStateManager.RoundState.CountMoney:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                inputHandler.enabled = false;
                break;
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                inputHandler.enabled = true;
                break;
        }
    }
}
