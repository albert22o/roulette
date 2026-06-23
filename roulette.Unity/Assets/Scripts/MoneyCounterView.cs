using System.Collections;
using TMPro;
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
    private Animator animator;
    private int currentDisplayedMoney;
    private int result;

    [Inject]
    public void Construct(BetCalculator betCalculator, ChipSpawner chipSpawner)
    {
        this.betCalculator = betCalculator;
        this.chipSpawner = chipSpawner;
    }

    private void Start()
    {
        betCalculator.CountMoney += OnCountMoney;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        betCalculator.CountMoney -= OnCountMoney;
    }

    private void OnCountMoney(int bet, int multiplier, int reward)
    {
        result = reward;
        currentDisplayedMoney = 0;
        resultText.text = "0";
        formulaText.text = $"{bet} x {multiplier}";
        animator.SetTrigger("fade");
    }

    public IEnumerator ProcessCounting()
    {
        while (currentDisplayedMoney < result)
        {
            currentDisplayedMoney += (int)Mathf.Lerp(0, result, 1 / timeToCountMoney * Time.fixedDeltaTime);
            resultText.text = Mathf.Clamp(currentDisplayedMoney, 0, result).ToString();

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(timeBeforeChipSpawn);

        chipSpawner.RequestSpawn(result / BetTable.BetStep);
        betCalculator.GetRewardAction();
        animator.SetTrigger("fade");

        yield return new WaitForSeconds(timeBeforeChipClear);

        chipSpawner.RecquestClear();
    }
}
