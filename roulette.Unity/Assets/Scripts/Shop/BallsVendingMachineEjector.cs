using System;
using UnityEngine;

public class BallsVendingMachineEjector : MonoBehaviour
{
    [SerializeField] private BallsVendingMachine ballsVendingMachine;
    [SerializeField] private BallsVendingMachineBall currentBall;
    private ItemData ballData;

    public event Action BallUpdated;

    private void OnEnable()
    {
        ballsVendingMachine.BallBought += OnBallBought;
    }

    private void OnBallBought(ItemData itemData)
    {
        ballData = itemData;
    }

    public void EjectBall()
    {
        currentBall.gameObject.SetActive(true);
        currentBall.SetVisual(ballData.VisualComponent);
        currentBall.transform.position = transform.position;
        BallUpdated?.Invoke();
    }
}
