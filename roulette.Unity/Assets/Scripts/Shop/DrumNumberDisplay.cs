using UnityEngine;

public class DrumNumberDisplay : MonoBehaviour
{
    [SerializeField] private DrumNumberWheel[] wheels;
    [SerializeField] private int number;

    private void OnValidate()
    {
        SetNumber(number, true);
    }

    private void Start()
    {
        SetNumber(number, true);
    }

    public void SetNumber(int number, bool setImmediately = false)
    {
        if (wheels == null || wheels.Length == 0) return;

        this.number = number;
        int maxNumber = (int)Mathf.Pow(10, wheels.Length) - 1;

        if (number > maxNumber)
        {
            Debug.LogWarning($"Число {number} превышает лимит! Максимум для {wheels.Length} колес: {maxNumber}. Отображаю максимум.");
            number = maxNumber;
        }
        for (int i = 0; i < wheels.Length; i++)
        {
            int digit = number % 10;
            wheels[i].SetDigit(digit, setImmediately);
            number /= 10;
        }
    }
}