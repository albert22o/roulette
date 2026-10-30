using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class WinningSequenceView : MonoBehaviour
{
    [SerializeField] DealerAnimation dealerAnimator;
    [SerializeField] private Animator dealerCollectAnimator;
    [SerializeField] float timeBeforeChipApply = 1f;
    [SerializeField] float timeBetweenChipAnimations = 0.5f;
    private WinningSequence winningSequence;
    private ChipHolder chipHolder;
    private BetCalculator betCalculator;
    private IAnimateBalls animateBalls;
    private BallHolder ballHolder;

    [Inject]
    public void Construct(WinningSequence winningSequence, BetCalculator betCalculator, ChipHolder chipHolder,
        IAnimateBalls animateBalls, BallHolder ballHolder)
    {
        this.winningSequence = winningSequence;
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.animateBalls = animateBalls;
        this.ballHolder = ballHolder;
    }

    private void Start()
    {
        winningSequence.SequenceStart += OnSequenceStart;
        winningSequence.ApplyChips += OnApplyChips;
        winningSequence.ApplyBall += OnApplyBall;
        winningSequence.DisplayMultiplier += OnDisplayMultiplier;
        winningSequence.EndSequence += OnEndSequence;
    }

    private void OnDestroy()
    {
        winningSequence.SequenceStart -= OnSequenceStart;
        winningSequence.ApplyChips -= OnApplyChips;
        winningSequence.ApplyBall -= OnApplyBall;
        winningSequence.DisplayMultiplier -= OnDisplayMultiplier;
        winningSequence.EndSequence -= OnEndSequence;
    }

    private void OnSequenceStart(int betSum, int _a1, float _a2)
    {
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.BetHighlight);
    }

    private void OnApplyChips(BetInfo betInfo, HitInfo hitInfo)
    {
        StartCoroutine(ApplyChips(betInfo, hitInfo));
    }

    private IEnumerator ApplyChips(BetInfo betInfo, HitInfo hitInfo)
    {
        yield return new WaitForSeconds(timeBeforeChipApply);

        for (int i = 0; i < chipHolder.Capacity; i++)
        {
            chipHolder.ApplyByIndex(i);
            winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.GetMaxWinningMultiplier(betInfo, hitInfo));

            yield return new WaitForSeconds(timeBetweenChipAnimations);
        }
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.CountMoney);
    }

    private void OnApplyBall(HitInfo hitInfo)
    {
        ballHolder.ApplySelectedBall(hitInfo.BallHash, hitInfo);
        animateBalls.AnimateBall(hitInfo.BallHash);
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyChips);
    }

    private void OnDisplayMultiplier(float multiplier)
    {
    }

    private void OnEndSequence(EndSequenceType endSequenceType)
    {
        if (endSequenceType == EndSequenceType.FinishBet ||
            endSequenceType == EndSequenceType.WinRound)
        {
            dealerCollectAnimator.SetTrigger("collect");
            dealerAnimator.PlayCollectAnimation();
        }
    }
}
