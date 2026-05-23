using UnityEngine;

public class DrumNumberDisplay : MonoBehaviour
{
    [SerializeField] private DrumNumberWheel[] wheels;
    [SerializeField] private int number;

    private void OnValidate()
    {
        SetNumber(number);
    }

    public void SetNumber(int number)
    {
        if (wheels == null || wheels.Length == 0) return;

        this.number = number;
        int maxNumber = (int)Mathf.Pow(10, wheels.Length) - 1;

        if (number > maxNumber)
        {
            Debug.LogWarning($"[DigitController] Число {number} превышает лимит! Максимум для {wheels.Length} колес: {maxNumber}. Отображаю максимум.");
            number = maxNumber;
        }
        for (int i = 0; i < wheels.Length; i++)
        {
            int digit = number % 10;
            wheels[i].CurrentDigit = digit;
            number /= 10;
        }
    }
}