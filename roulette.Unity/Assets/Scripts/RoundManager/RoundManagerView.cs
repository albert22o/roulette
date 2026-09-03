using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoundManagerView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI quoteText;
    [SerializeField] TextMeshProUGUI betCountText;
    [SerializeField] TextMeshProUGUI rerollCountText;

    [SerializeField] RouletteClickHandler rouletteClickHandler;
    [SerializeField] TableRerollButton rerollButton;
    [SerializeField] Button confirmWinButton;

    [SerializeField] Canvas canvas;
    private RoundManager roundManager;

    [Inject]
    public void Construct(RoundManager roundManager, Camera camera)
    {
        this.roundManager = roundManager;
        canvas.worldCamera = camera;
    }

    public void Start()
    {
        if (quoteText == null || betCountText == null || rerollCountText == null)
        {
            Debug.LogError("Не все зависимости проставлены!");
            return;
        }

        roundManager.BetCountChanged += OnBetCountChanged;
        roundManager.QuoteChanged += OnQuoteOrGoalChanged;
        roundManager.RerollCountChanged += OnRerollCountChanged;
        roundManager.GoalChanged += OnQuoteOrGoalChanged;
        roundManager.AbleToWinChanged += OnAbleToWinChanged;

        OnBetCountChanged(roundManager.BetCount);
        OnQuoteOrGoalChanged(roundManager.Quote);
        OnRerollCountChanged(roundManager.RerollCount);
        OnAbleToWinChanged(roundManager.AbleToWin);
        

        rouletteClickHandler.Clicked += OnSpinButtonClick;
        rerollButton.Clicked += OnRerollButtonClick;
        confirmWinButton.onClick.AddListener(OnConfirmWinClick);
    }

    private void OnConfirmWinClick()
    {
        roundManager.ConfirmWin();
        confirmWinButton.gameObject.SetActive(false);   
    }

    private void OnSpinButtonClick()
    {
        roundManager.Spin();
    }

    private void OnRerollButtonClick()
    {
        roundManager.Reroll();
    }

    private void OnRerollCountChanged(int newRerollCount)
    {
        rerollCountText.text = newRerollCount.ToString();
    }

    private void OnQuoteOrGoalChanged(int _)
    {
        quoteText.text = $"{roundManager.Quote}/{roundManager.Goal}";
    }

    private void OnAbleToWinChanged(bool ableToWin)
    {
        confirmWinButton.gameObject.SetActive(ableToWin);
    }

    private void OnBetCountChanged(int newBet)
    {
        betCountText.text = newBet.ToString();
    }
}