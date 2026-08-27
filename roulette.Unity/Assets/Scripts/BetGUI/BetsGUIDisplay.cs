using System.Collections.Generic;
using UnityEngine;

public class BetsGUIDisplay : MonoBehaviour
{
    [SerializeField] private BetDisplayItem betItemPrefab;  
    [SerializeField] private Transform container;

    private readonly List<BetDisplayItem> dots = new();

    public void SetRemainingBets(int remainingBets, int maxBets)
    {
        if (dots.Count != maxBets)
        {
            RebuildDots(maxBets);
        }

        for (int i = 0; i < dots.Count; i++)
        {
            bool isRemaining = i < remainingBets;
            dots[i].SetBet(isRemaining);
        }
    }

    private void RebuildDots(int maxBets)
    {
        while (dots.Count < maxBets)
        {
            BetDisplayItem dot = Instantiate(betItemPrefab, container);
            dot.transform.SetAsFirstSibling();
            dots.Add(dot);
        }
        while (dots.Count > maxBets)
        {
            var lastDot = dots[dots.Count - 1];
            dots.RemoveAt(dots.Count - 1);
            Destroy(lastDot.gameObject);
        }
    }
}
