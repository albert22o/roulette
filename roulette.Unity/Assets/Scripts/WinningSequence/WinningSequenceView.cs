using System.Collections;
using TMPro;
using UnityEditor.Recorder;
using UnityEngine;
using Zenject;

public class WinningSequenceView : MonoBehaviour
{
    [SerializeField] TextMeshPro betText;
    [SerializeField] TextMeshPro generalMultiplierText;
    [SerializeField] Animator dealerAnimator;
    [SerializeField] float timeBeforeChipApply = 1f;
    [SerializeField] float timeBetweenChipAnimations = 0.5f;

    private WinningSequence winningSequence;
    private ChipHolder chipHolder;
    private BetCalculator betCalculator;
    private IAnimateBalls animateBalls;

    [Inject]
    public void Construct(WinningSequence winningSequence, BetCalculator betCalculator, ChipHolder chipHolder,
        IAnimateBalls animateBalls)
    {
        this.winningSequence = winningSequence;
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.animateBalls = animateBalls;
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

    private void OnSequenceStart(int betSum, int _)
    {
        betText.text = betSum.ToString();
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
        animateBalls.AnimateBall(hitInfo.BallHash);
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyChips);
    }

    private void OnDisplayMultiplier(int multiplier)
    {
        generalMultiplierText.text = multiplier.ToString();
    }

    private void OnEndSequence(bool isWin)
    {
        generalMultiplierText.text = "";
        betText.text = "";
        if (isWin)
            dealerAnimator.SetTrigger("collect");
    }
}
