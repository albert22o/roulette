using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BallResultsUI : MonoBehaviour
{
    [SerializeField] BallResultCardUI ballResultCardPrefab;
    [SerializeField] Transform ballResultsContainer;

    private readonly List<BallResultCardUI> cards = new();
    private BallResults ballResults;

    [Inject]
    public void Construct(BallResults ballResults)
    {
        this.ballResults = ballResults;
    }

    public void Init()
    {
        ballResults.BallAdded += OnBallAdded;
        ballResults.BallRemoved += OnBallRemoved;
    }

    private void OnBallRemoved(VisualComponent component, HitInfo info)
    {
        var card = cards.Find(card => card.HitInfo == info);
        Destroy(card);
        cards.Remove(card);
    }

    private void OnBallAdded(VisualComponent component, HitInfo info)
    {
        var ballCard = Instantiate(ballResultCardPrefab, ballResultsContainer);
        ballCard.Init(info, () => ballResults.SelectBall(info));
        cards.Add(ballCard);
    }
}
