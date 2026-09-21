using UnityEngine;
using Zenject;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] private int currencyAmount = 100;
    private GoldenChips goldenChips;
    private BossChips bossChips;
    private BetTable betTable;
    [Inject] private void Construct(GoldenChips goldenChips, BossChips bossChips, BetTable betTable)
    {
        this.goldenChips = goldenChips;
        this.bossChips = bossChips;
    }
    public void AddGoldenChips()
    {
        goldenChips.Add(currencyAmount);
    }

    public void AddBossChips()
    {
        bossChips.Add(currencyAmount);
    }

    public void AddSilverChips()
    {
        betTable.AddAvailableSum(currencyAmount);
    }
}
