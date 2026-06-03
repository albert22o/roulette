using UnityEngine;
using System.Collections;
using Zenject;

public class CurrencyDisplayUI : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private CurrencyCounterUI[] counters;

    [Header("Test Values")]
    [SerializeField] private int testGoldAmount = 0;
    [SerializeField] private int testBossAmount = 0;
    [SerializeField] private int testTicketAmount = 0;
    [SerializeField] private int testMoneyAmount = 0;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private bool isShowing = false;
    [SerializeField] private float hideAnimationDuration = 0.3f;


    private GoldenChips goldenChips;
    private BossChips bossChips;
    private Tickets tickets;
    private Money money;


    [Inject]
    public void Construct(GoldenChips goldenChips, BossChips bossChips, Tickets tickets, Money money)
    {
        this.goldenChips = goldenChips;
        this.bossChips = bossChips;
        this.tickets = tickets;
        this.money = money;
    }

    private void Start()
    {
        goldenChips.AmountChanged += counters[0].UpdateCounter;
        bossChips.AmountChanged += counters[1].UpdateCounter;
        tickets.AmountChanged += counters[2].UpdateCounter;
        money.AmountChanged += counters[3].UpdateCounter;
    }

    public void ShowTemporarily()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        animator.SetBool("isShowing", true);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        animator.SetBool("isShowing",false);
        //yield return new WaitForSeconds(hideAnimationDuration);
        hideCoroutine = null;
    }
}