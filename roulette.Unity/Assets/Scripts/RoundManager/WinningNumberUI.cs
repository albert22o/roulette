using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class WinningNumberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private MeshRenderer textMeshRenderer;
    [SerializeField] private Material winningNumberMaterial;

    private RouletteWheel rouletteWheel;
    private BallResults ballResults;
    private RoundStateManager roundStateManager;

    private Material originalMaterial;
    private bool isBallSelected = false;
    private Dictionary<string, HitInfo> hitsByBallHash = new Dictionary<string, HitInfo>();

    [Inject]
    public void Construct(RouletteWheel rouletteWheel, BallResults ballResults, RoundStateManager roundStateManager)
    {
        this.rouletteWheel = rouletteWheel;
        this.ballResults = ballResults;
        this.roundStateManager = roundStateManager;
    }

    private void Start()
    {
        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
            numberText.text = "";
            numberText.raycastTarget = false;
        }

        if (textMeshRenderer != null)
        {
            originalMaterial = textMeshRenderer.material;
        }
    }

    private void OnEnable()
    {
        if (rouletteWheel != null)
        {
            rouletteWheel.BallHover += OnBallHoveredWithHash;
            rouletteWheel.BallUnHover += OnBallUnHovered;
            rouletteWheel.BallClicked += OnBallClicked;
            rouletteWheel.AllBallsStopped += OnBallsStopped;
            rouletteWheel.Rerolled += OnReroll;
        }

        if (roundStateManager != null)
        {
            roundStateManager.StateChanged += OnRoundStateChanged;
        }
    }

    private void OnDisable()
    {
        if (rouletteWheel != null)
        {
            rouletteWheel.BallHover -= OnBallHoveredWithHash;
            rouletteWheel.BallUnHover -= OnBallUnHovered;
            rouletteWheel.BallClicked -= OnBallClicked;
            rouletteWheel.AllBallsStopped -= OnBallsStopped;
            rouletteWheel.Rerolled -= OnReroll;
        }

        if (roundStateManager != null)
        {
            roundStateManager.StateChanged -= OnRoundStateChanged;
        }
    }

    private void OnReroll()
    {
        Hide();
    }

    private void OnBallsStopped(HitInfo[] hitsInfo)
    {
        hitsByBallHash.Clear();
        foreach (var hit in hitsInfo)
        {
            if (hit != null && !string.IsNullOrEmpty(hit.BallHash))
            {
                hitsByBallHash[hit.BallHash] = hit;
            }
        }
    }

    private void OnBallHoveredWithHash(string ballHash)
    {
        if (isBallSelected) return;

        if (hitsByBallHash.TryGetValue(ballHash, out HitInfo hitInfo))
        {
            ShowNumber(hitInfo.FieldNumber);
        }
    }

    private void OnBallUnHovered(string ballHash)
    {
        if (!isBallSelected)
        {
            Hide();
        }
    }

    private void OnBallClicked(string ballHash)
    {
        if (hitsByBallHash.TryGetValue(ballHash, out HitInfo hitInfo))
        {
            isBallSelected = true;
            ShowNumber(hitInfo.FieldNumber);
        }
    }

    private void OnRoundStateChanged(RoundStateManager.RoundState newState)
    {

        if (newState == RoundStateManager.RoundState.WheelSpining ||
            newState == RoundStateManager.RoundState.PlacingBets ||
            newState == RoundStateManager.RoundState.Start)
        {
            hitsByBallHash.Clear();
            isBallSelected = false;
            Hide();
        }

        if (newState == RoundStateManager.RoundState.BetResults ||
            newState == RoundStateManager.RoundState.NotActive)
        {
            isBallSelected = false;
            Hide();
        }
    }

    private void ShowNumber(int number)
    {
        if (numberText == null) return;

        numberText.gameObject.SetActive(true);
        numberText.text = number.ToString();

        if (textMeshRenderer != null && winningNumberMaterial != null)
        {
            textMeshRenderer.material = winningNumberMaterial;
        }
    }

    public void Hide()
    {
        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
        }

        if (textMeshRenderer != null && originalMaterial != null)
        {
            textMeshRenderer.material = originalMaterial;
        }
    }
}