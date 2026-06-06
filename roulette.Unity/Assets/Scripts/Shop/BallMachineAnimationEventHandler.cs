using UnityEngine;

public class BallMachineAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private BallsVendingMachineEjector ballEjector;
    public void EjectBall()
    {
        ballEjector.EjectBall();
    }
}
