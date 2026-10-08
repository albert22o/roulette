using System;
using UnityEngine;

public class DealerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private readonly int collectTrigger = Animator.StringToHash("Collect");
    private readonly int gameOverTrigger = Animator.StringToHash("GameOver");

    public event Action GameOverAnimationFinished;
    public event Action KillPlayer;

    public void PlayCollectAnimation()
    {
        animator.SetTrigger(collectTrigger);
    }

    public void PlayGameOverAnimation()
    {
        animator.SetTrigger(gameOverTrigger);
    }

    public void OnGameOverAnimationFinish()
    {
        GameOverAnimationFinished?.Invoke();
    }

    public void OnKillPlayer()
    {
        KillPlayer?.Invoke();
    }
}
