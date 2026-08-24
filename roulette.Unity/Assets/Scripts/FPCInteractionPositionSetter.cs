using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FPCInteractionPositionSetter : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private FirstPersonController fpc;

    private GameStateManager gameStateManager;
    [SerializeField] private GameStateManager.GameState requiredState = GameStateManager.GameState.BettingСycle;

    [Inject]
    private void Construct(FirstPersonController fpc, GameStateManager gameStateManager)
    {
        this.fpc = fpc;
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        if (gameStateManager.CurrentState == requiredState)
        {
            ChangePlayerPositionAndRotation();
        }
    }
    public void ChangePlayerPositionAndRotation()
    {
        if (cinemachineCamera != null)
        {
            cinemachineCamera.gameObject.SetActive(true);
        }
        Vector3 camEuler = cinemachineCamera.transform.rotation.eulerAngles;
        float pitch = camEuler.x;

        if (pitch > 180f) pitch -= 360f;


        fpc.SetPositionAndRotation(transform.position, transform.rotation, pitch);
    }
}
