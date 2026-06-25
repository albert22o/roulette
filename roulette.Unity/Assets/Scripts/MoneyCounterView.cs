using System.Collections;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using Zenject;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[RequireComponent(typeof(Animator))]
public class MoneyCounterView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI formulaText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private float timeToCountMoney;
    [SerializeField] private float timeBeforeChipSpawn;
    [SerializeField] private float timeBeforeChipClear;

    private BetCalculator betCalculator;
    private ChipSpawner chipSpawner;
    private WinningSequence winningSequence;
    private Animator animator;
    private int currentDisplayedMoney;
    private int bet;
    private int reward;

    [Inject]
    public void Construct(BetCalculator betCalculator, ChipSpawner chipSpawner, WinningSequence winningSequence)
    {
        this.betCalculator = betCalculator;
        this.chipSpawner = chipSpawner;
        this.winningSequence = winningSequence;
    }

    private void Start()
    {
        winningSequence.CountMoney += OnCountMoney;
        winningSequence.DisplayMultiplier += OnDisplayMultiplier;
        winningSequence.SequenceStart += OnSequenceStart;
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

        animator.SetTrigger("fade");
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.Finish);

        //Сейчас нет анимации того как монстр сгребает фишки, поэтому они просто исчезают после времени
        yield return new WaitForSeconds(timeBeforeChipClear);

        chipSpawner.RequestClear();
    }

    private void OnSequenceStart(int bet, int multiplier)
    {
        this.bet = bet;
        resultText.text = "0";
        formulaText.text = $"{bet} x {multiplier}";
        animator.SetTrigger("fade");
    }

    private void OnDisplayMultiplier(int multiplier)
    {
        formulaText.text = $"{bet} x {multiplier}";
    }
}
