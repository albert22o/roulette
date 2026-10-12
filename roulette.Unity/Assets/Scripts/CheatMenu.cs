using UnityEngine;
using Zenject;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] private int currencyAmount = 100;
    private GoldenChips goldenChips;
    private BossChips bossChips;
    private BetTable betTable;
    private CursorManager cursorManager;

    [Inject] private void Construct(GoldenChips goldenChips, BossChips bossChips,
        BetTable betTable, CursorManager cursorLockMode)
    {
        this.goldenChips = goldenChips;
        this.bossChips = bossChips;
        this.betTable = betTable;
        this.cursorManager = cursorLockMode;
    }

    private void OnEnable()
    {
        cursorManager.ShowCursorRequest();
    }

    private void OnDisable()
    {
        cursorManager.HideCursorRequest();
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
