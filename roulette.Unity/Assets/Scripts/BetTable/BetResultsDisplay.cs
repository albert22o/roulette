using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class BetResultsDisplay : MonoBehaviour
{
    private RoundStateManager roundStateManager;
    public event Action<int> GetReward;

    [Inject]
    public void Construct(RoundStateManager roundStateManager, BetCalculator betCalculator)
    {
        this.roundStateManager = roundStateManager;
        betCalculator.DisplayBetResults += StartResultDisplaySequence;
    }

    public void StartResultDisplaySequence(int reward)
    {
        StartCoroutine(PeekOnBets(reward));
    }

    private IEnumerator PeekOnBets(int reward)
    {
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);

        yield return new WaitForSeconds(4f);

        GetReward?.Invoke(reward);
    }
}
