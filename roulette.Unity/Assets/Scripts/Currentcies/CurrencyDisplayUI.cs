using UnityEngine;
using TMPro;
using System.Collections;

public class CurrencyDisplayUI : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private CurrencyCounterUI[] counters;

    [Header("Ые Values")]
    [SerializeField] private int testGoldAmount = 0;
    [SerializeField] private int testBossAmount = 0;
    [SerializeField] private int testTicketAmount = 0;
    [SerializeField] private int testMoneyAmount = 0;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;

    private GoldenChips goldenChips;
    private BossChips bossChips;
    private Tickets tickets;
    private Money money;

    private void Awake()
    {
        // Создаём ресурсы
        goldenChips = new GoldenChips();
        bossChips = new BossChips();
        tickets = new Tickets();
        money = new Money();
        goldenChips.Add(testGoldAmount);
        bossChips.Add(testBossAmount);
        tickets.Add(testTicketAmount);
        money.Add(testMoneyAmount);
        
    }

    public void Construct(GoldenChips goldenChips, BossChips bossChips, Tickets tickets, Money money)
    {
        this.goldenChips = goldenChips;
        this.bossChips = bossChips;
        this.tickets = tickets;
        this.money = money;
    }

    private void Start()
    {
        {
            goldenChips.AmountChanged += counters[0].UpdateCounter;
            bossChips.AmountChanged += counters[1].UpdateCounter;
            tickets.AmountChanged += counters[2].UpdateCounter;
            money.AmountChanged += counters[3].UpdateCounter;
        }
    }

    private void OnDisable()
    {

        goldenChips.AmountChanged -= counters[0].UpdateCounter;
        bossChips.AmountChanged -= counters[1].UpdateCounter;
        tickets.AmountChanged -= counters[2].UpdateCounter;
        money.AmountChanged -= counters[3].UpdateCounter;
    }

    public void ShowTemporarily()
    {

        counters[0].UpdateCounter(goldenChips.Amount);
        counters[1].UpdateCounter(bossChips.Amount);
        counters[2].UpdateCounter(tickets.Amount);
        counters[3].UpdateCounter(money.Amount);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideAll();
        hideCoroutine = null;
    }

    private void HideAll()
    {
        foreach (var counter in counters)
        {
            counter.ForceHide();
        }
    }
}