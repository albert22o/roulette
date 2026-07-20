using System;
using UnityEngine;

public class BallsVendingMachineEjector : MonoBehaviour
{
    [SerializeField] private BallsVendingMachine ballsVendingMachine;
    [SerializeField] private BallsVendingMachineBall currentBall;
    private BallInfoData ballData;

    public event Action BallUpdated;

    private void OnEnable()
    {
        ballsVendingMachine.BallBought += OnBallBought;
    }

    private void OnBallBought(BallInfoData itemData)
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
