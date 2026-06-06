using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BallsVendingMachineSelectionView : MonoBehaviour
{
    [SerializeField] private Transform viewPoint, hidePoint;
    [SerializeField] private float animationDuration;
    [SerializeField] private BallsVendingMachine ballsMachine;
    [SerializeField] private BallsVendingMachineEjector ballsEjector;
    [SerializeField] private float timeToPickupBall = 1f;
    [SerializeField] private Canvas ballMachineCanvas;
    [SerializeField] private TextMeshProUGUI itemName, itemDescription;
    [SerializeField] private Shop shop;
    [SerializeField] private Animator ballMachineAnimator;

    [SerializeField] private BallsVendingMachineBall ball;
    private void OnEnable()
    {
        ballsEjector.BallUpdated += OnBallSpawned;
        ballsMachine.BallBought += UpdateText;
    }
    private void OnDisable()
    {
        ballsEjector.BallUpdated -= OnBallSpawned;
        ballsMachine.BallBought -= UpdateText;
    }
    
    private void UpdateText(ItemData data)
    {
        itemName.text = data.NameKey;
        itemDescription.text = data.DescriptionKey;
    }

    private void OnBallSpawned()
    {
        StartCoroutine(PickupBall());
    }

    private IEnumerator PickupBall()
    {
        yield return new WaitForSeconds(timeToPickupBall);
        ballMachineAnimator.enabled = false;
        ball.transform.DOMove(viewPoint.position, animationDuration).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            ballMachineCanvas.gameObject.SetActive(true);
        });
    }

    public void Sell()
    {
        ballsMachine.SellBall();
        ballMachineCanvas.gameObject.SetActive(false);
        ball.gameObject.SetActive(false);
    }

    public void Apply()
    {
        ballMachineCanvas.gameObject.SetActive(false);
        ball.transform.DOMove(hidePoint.position, animationDuration).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            ballsMachine.AddBallToInventory();
            ball.gameObject.SetActive(false);
        });
    }

}
