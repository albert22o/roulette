using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoundManagerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI balanceText;
    [SerializeField] TextMeshProUGUI betCountText;
    [SerializeField] TextMeshProUGUI rerollCountText;
    
    [SerializeField] Button spinButton;
    [SerializeField] Button rerollButton;

    private RoundManager roundManager;

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        this.roundManager = roundManager;
    }

    public void Init()
    { 
    }
}

