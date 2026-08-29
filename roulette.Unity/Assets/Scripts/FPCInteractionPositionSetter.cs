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
        Vector3 camEuler = cinemachineCamera.transform.rotation.eulerAngles;
        float pitch = camEuler.x;

        if (pitch > 180f) pitch -= 360f;


        fpc.SetPositionAndRotation(transform.position, transform.rotation, pitch);
    }
}
