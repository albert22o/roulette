using UnityEngine;

public class BallsVendingMachineVisual : MonoBehaviour
{
    private static readonly int EjectBallHash = Animator.StringToHash("EjectBall");
    [SerializeField] private Animator animator;
    [SerializeField] private BallsVendingMachine ballsMachine;
    
    private void OnEnable()
    {
        ballsMachine.BallBought += OnBallBought;
    }

    private void OnBallBought(ItemData itemData)
    {
        PlayEjectBallAnimation();
    }

    public void PlayEjectBallAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger(EjectBallHash);
    }
}
