using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class GameOverView : MonoBehaviour
{
    [Header("Scene refs")]
    [SerializeField] private CinemachineCamera defeatCamera;
    [SerializeField] private CinemachineCamera deathCamera, approachCamera;
    [SerializeField] private DealerAnimation dealerAnimation;

    [Header("GameOver prefab (children)")]
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    [Header("Material WinMat")]
    [SerializeField] private Material winMat;
    [SerializeField] private string powerProperty = "_Power";
    [SerializeField] private string blackProperty = "_Black";

    [Header("Timings")]
    [SerializeField] private float powerDuration = 1.5f;
    [SerializeField] private float blackDuration = 1.5f;
    [SerializeField] private float powerFrom = 100f;
    [SerializeField] private float powerTo = 20f;
    [SerializeField] private float blackFrom = -1f;
    [SerializeField] private float blackTo = 0f;


    private int powerId;
    private int blackId;
    private bool isPlaying;
    private bool isWin;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.Lose)
        {
            isWin = false;
            ShowEnding(false);
        }
        else if (newState == GameStateManager.GameState.Win)
        {
            isWin = true;
            ShowEnding(true);
        }
    }

    private void Awake()
    {
        powerId = Shader.PropertyToID(powerProperty);
        blackId = Shader.PropertyToID(blackProperty);

        winMat.SetFloat(powerId, powerFrom);
        winMat.SetFloat(blackId, blackFrom);
        ResetMaterial();

        gameOverRoot.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);

        dealerAnimation.GameOverAnimationFinished += OnGameOverAnimationFinished;
        dealerAnimation.KillPlayer += OnKillPlayer;
        dealerAnimation.ApproachPlayer += OnApproachPlayer;
    }

    private void OnKillPlayer()
    {
        approachCamera.gameObject.SetActive(false);
        deathCamera.gameObject.SetActive(true);
    }

    private void OnApproachPlayer()
    {
        defeatCamera.gameObject.SetActive(false);
        approachCamera.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        ResetMaterial();
    }

    private void OnGameOverAnimationFinished()
    {
        StartCoroutine(ContinueGameOver()); 
    }

    private IEnumerator ContinueGameOver()
    {
        Coroutine black = StartCoroutine(LerpMaterial(blackId, blackFrom, blackTo, blackDuration));
        Coroutine power = StartCoroutine(LerpMaterial(powerId, powerFrom, powerTo, powerDuration));

        yield return black;
        yield return power;

        gameOverRoot.SetActive(true);
        winText.SetActive(isWin);
        loseText.SetActive(!isWin);
    }


    public void ShowEnding(bool isWin)
    {
        if (isPlaying) return;
        
        isPlaying = true;
        dealerAnimation.PlayGameOverAnimation();
        defeatCamera.gameObject.SetActive(true);
    }


    private IEnumerator LerpMaterial(int propertyId, float from, float to, float duration)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
        {
            winMat.SetFloat(propertyId, Mathf.Lerp(from, to, t));
            yield return null;
        }
        winMat.SetFloat(propertyId, to);
    }

    private void ResetMaterial()
    {
        if (winMat == null) return;
        winMat.SetFloat(powerId, powerFrom);
        winMat.SetFloat(blackId, blackFrom);
    }
}

