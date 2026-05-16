using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;

    private RoundStateManager roundStateManager;

    [Inject]
    public void Construct(RoundStateManager roundStateManager)
    {
        this.roundStateManager = roundStateManager;
    }

    public void Init()
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
                break;
            case RoundStateManager.RoundState.WheelSpining:
            case RoundStateManager.RoundState.ChoosingReward:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                break;
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                break;
        }
    }
}
