using System;

[Serializable]
public class GoldenChips : ICurrencyResource
{
    public int Amount { get; private set; }

    public event Action<int> AmountChanged;

    public void Add(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "amount must be greater than zero.");

        Amount += amount;
        AmountChanged?.Invoke(Amount);
    }

    public void Remove(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "amount must be greater than zero.");

        Amount -= amount;
        AmountChanged?.Invoke(Amount);
    }
}