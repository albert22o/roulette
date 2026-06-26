using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class MoneyCounterView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI formulaText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private float timeToCountMoney;
    [SerializeField] private float timeBeforeChipSpawn;
    [SerializeField] private float timeBeforeChipClear;

    private ChipSpawner chipSpawner;
    private WinningSequence winningSequence;
    private Animator animator;
    private int currentDisplayedMoney;
    private int bet;
    private int reward;

    [Inject]
    public void Construct(ChipSpawner chipSpawner, WinningSequence winningSequence)
    {
        this.chipSpawner = chipSpawner;
        this.winningSequence = winningSequence;
    }

    private void Start()
    {
        winningSequence.CountMoney += OnCountMoney;
        winningSequence.DisplayMultiplier += OnDisplayMultiplier;
        winningSequence.SequenceStart += OnSequenceStart;
        winningSequence.EndSequence += OnEndSequence;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        winningSequence.CountMoney -= OnCountMoney;
    }

    private void OnCountMoney(int bet, int multiplier, int reward)
    {
        this.reward = reward;
        currentDisplayedMoney = 0;
        animator.SetTrigger("collapse");
    }

    public IEnumerator ProcessCounting()
    {
        while (currentDisplayedMoney < reward)
        {
            currentDisplayedMoney += (int)Mathf.Lerp(0, reward, 1 / timeToCountMoney * Time.fixedDeltaTime);
            resultText.text = Mathf.Clamp(currentDisplayedMoney, 0, reward).ToString();

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(timeBeforeChipSpawn);

        chipSpawner.RequestSpawn(reward / BetTable.BetStep);
    }

    private void OnSequenceStart(int bet, int multiplier)
    {
        this.bet = bet;
        resultText.text = "0";
        formulaText.text = $"{bet} x {multiplier}";
        animator.SetTrigger("fade");
    }

    private void OnEndSequence(bool _)
    {
        animator.SetTrigger("fade");
    }

    private void OnDisplayMultiplier(int multiplier)
    {
        formulaText.text = $"{bet} x {multiplier}";
    }
}
