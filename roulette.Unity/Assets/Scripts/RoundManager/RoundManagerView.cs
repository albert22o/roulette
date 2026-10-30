using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class RoundManagerView : MonoBehaviour
{
    [SerializeField] RouletteClickHandler rouletteClickHandler;
    [SerializeField] TableRerollButton rerollButton;
    [SerializeField] TableWinButton confirmWinButton;
    [SerializeField] TextMeshProUGUI quoteNum;

    [SerializeField] Canvas canvas;
    private ProgressionManager progressionManager;
    private RoundManager roundManager;

    [Inject]
    public void Construct(RoundManager roundManager, Camera camera, ProgressionManager progressionManager)
    {
        this.progressionManager = progressionManager;
        this.roundManager = roundManager;
        canvas.worldCamera = camera;
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        quoteNum.text = Loc.Get("progress.completed_quotas", progressionManager.CurrentIndex);
    }

    public void Start()
    {
        roundManager.AbleToWinChanged += OnAbleToWinChanged;
        quoteNum.text = Loc.Get("progress.completed_quotas", progressionManager.CurrentIndex);

        OnAbleToWinChanged(roundManager.AbleToWin);
        

        rouletteClickHandler.Clicked += OnSpinButtonClick;
        rerollButton.Clicked += OnRerollButtonClick;
        confirmWinButton.Clicked += OnConfirmWinClick;
    }

    private void OnConfirmWinClick()
    {
        roundManager.ConfirmWin(); 
        quoteNum.text = Loc.Get("progress.completed_quotas", progressionManager.CurrentIndex);
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
    }
}