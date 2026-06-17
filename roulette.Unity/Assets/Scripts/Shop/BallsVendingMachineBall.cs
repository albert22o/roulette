using UnityEngine;

public class BallsVendingMachineBall : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BallVisual ballVisual;

    public void SetVisual(VisualComponent visual)
    {
        ballVisual.SetVisualComponent(visual);
    }

    public void DisablePhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}
