using System;
using UnityEngine;
using Zenject;

public class DealerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private SFXManager SfxManager;
    private readonly int collectTrigger = Animator.StringToHash("Collect");
    private readonly int gameOverTrigger = Animator.StringToHash("GameOver");

    public event Action GameOverAnimationFinished;
    public event Action KillPlayer, ApproachPlayer;

    [Inject]
    public void Construct(SFXManager SfxManager)
    {
        this.SfxManager = SfxManager;
    }

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

    public void OnApproachPlayer()
    {
        ApproachPlayer?.Invoke();
    }

    public void OnStartCutscene()
    {
        SfxManager.Play("CreepyMusic");
    }

    public void OnPlaySound()
    {
        SfxManager.Play("HitTheTable");
    }

    public void OnKillPlayer()
    {
        KillPlayer?.Invoke();
    }
}
