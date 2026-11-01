using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FPCInteractionPositionSetter : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private FirstPersonController fpc;

    [Inject]
    public void Construct(FirstPersonController fpc)
    {
        this.fpc = fpc;
    }

    public void ChangePlayerPositionAndRotation()
    {
        fpc.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
