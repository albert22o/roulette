using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoundManagerView : MonoBehaviour
{
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
        roundManager.AbleToWinChanged += OnAbleToWinChanged;

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

    private void OnAbleToWinChanged(bool ableToWin)
    {
        confirmWinButton.gameObject.SetActive(ableToWin);
    }
}