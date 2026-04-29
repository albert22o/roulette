using UnityEngine;
using Zenject;

public class RoundManagerUI : MonoBehaviour
{
    [SerializeField] GameObject choiceButtonPrefab;
    [SerializeField] GameObject colorButtonPrefab;

    private RoundManager roundManager;

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        this.roundManager = roundManager;
    }
}

