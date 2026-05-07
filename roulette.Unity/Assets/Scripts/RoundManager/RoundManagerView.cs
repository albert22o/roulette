using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoundManagerView : MonoBehaviour
{
    [SerializeField] TextMeshPro quoteText;
    [SerializeField] TextMeshPro betCountText;
    [SerializeField] TextMeshPro rerollCountText;

    [SerializeField] TableButton spinButton;
    [SerializeField] TableButton rerollButton;

    private RoundManager roundManager;

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        this.roundManager = roundManager;
    }

    public void Init()
    {
        if (quoteText == null || betCountText == null || rerollCountText == null)
        {
            Debug.LogError("Не все зависимости проставлены!");
            return;
        }

        roundManager.BetCountChanged += OnBetCountChanged;
        roundManager.QuoteChanged += OnQuoteChanged;
        roundManager.RerollCountChanged += OnRerollCountChanged;

        OnBetCountChanged(roundManager.BetCount);
        OnQuoteChanged(roundManager.Quote);
        OnRerollCountChanged(roundManager.RerollCount);

        if (rerollButton == null || spinButton == null)
        {
            Debug.LogError("Не все зависимости проставлены!");
            return;
        }

        spinButton.Clicked += OnSpinButtonClick;
        rerollButton.Clicked += OnRerollButtonClick;
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

    private void OnQuoteChanged(int newQuote)
    {
        quoteText.text = $"{newQuote}/{roundManager.Goal}";
    }

    private void OnBetCountChanged(int newBet)
    {
        betCountText.text = newBet.ToString();
    }
}