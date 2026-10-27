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
    private float rerollCoeff;

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

    private void OnCountMoney(int bet, float _, int reward)
    {
        this.reward = reward;
        currentDisplayedMoney = 0;
        animator.SetTrigger("collapse");
    }

    public IEnumerator ProcessCounting()
    {
        winningSequence.NotifyMoneyCountStarted(timeToCountMoney);
        while (currentDisplayedMoney < reward)
        {
            currentDisplayedMoney += (int)Mathf.Lerp(0, reward, 1 / timeToCountMoney * Time.fixedDeltaTime);
            resultText.text = Mathf.Clamp(currentDisplayedMoney, 0, reward).ToString();

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(timeBeforeChipSpawn);

        chipSpawner.RequestSpawn(reward);
    }

    private void OnSequenceStart(int bet, int multiplier, float rerollCoeff)
    {
        this.bet = bet;
        this.rerollCoeff = rerollCoeff;
        resultText.text = "0";
        formulaText.text = $"{bet} x {multiplier:0.##} x {rerollCoeff:0.##}";
        animator.SetTrigger("fade");
    }

    private void OnEndSequence(EndSequenceType endSequenceType)
    {
        animator.SetTrigger("fade");
        chipSpawner.RequestClear();
    }

    private void OnDisplayMultiplier(float multiplier)
    {
        formulaText.text = $"{bet} x {multiplier:0.##} x {rerollCoeff:0.##}";
    }
}
