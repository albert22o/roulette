using System.Collections.Generic;
using UnityEngine;
using Zenject;

class BetTableUI : MonoBehaviour
{
    [SerializeField] Transform betButtonContainer;
    [SerializeField] BetButton zeroBetButton;
    [SerializeField] BetButton betButtonPrefab;

    private BetTable betTable;
    private readonly List<BetButton> betButtons = new();

    [Inject]
    public void Construct(BetTable betTable)
    {
        this.betTable = betTable;
    }

    public void Init()
    {
        for (int index = 0; index < betButtons.Count; index++)
        {
            if (index == 0)
            {
                zeroBetButton.Init(betTable.PocketMapping[index],
                    () => betTable.AddBet(index),
                    () => betTable.RemoveBet(index));
            }
            else
            {
                var betButton = Instantiate(betButtonPrefab, betButtonContainer);
                betButton.Init(betTable.PocketMapping[index],
                    () => betTable.AddBet(index),
                    () => betTable.RemoveBet(index));
                betButtons.Add(betButton);
            }
        }
    }
}