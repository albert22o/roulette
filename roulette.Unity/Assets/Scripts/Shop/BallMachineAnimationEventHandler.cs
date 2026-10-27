using UnityEngine;

public class BallMachineAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private BallsVendingMachineEjector ballEjector;
    [SerializeField] private BallsVendingMachine ballMachine;
    public void EjectBall()
    {
        ballEjector.EjectBall();
    }

    public void PlaySound()
    {
        ballMachine.PlayEjectBallSound();
    }

    public void PlayBallFellSound()
    {
        ballMachine.PlayBallFellSound();
    }
}
