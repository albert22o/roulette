using System;

public abstract class CurrencyResource
{
    public int Amount { get; private set; }

    public int OverdraftLimit { get; set; }
    public int AvailableAmount => Amount + OverdraftLimit;

    public event Action<int> AmountChanged;

    public void Add(int amount)
    {
        Amount += amount;
        AmountChanged?.Invoke(Amount);
    }

    public void Remove(int amount)
    {
        int newAmount = Amount - amount;
        if (newAmount < -OverdraftLimit)
            throw new InvalidOperationException(
                $"Cannot remove {amount}: resulting amount {newAmount} would exceed overdraft limit {OverdraftLimit}.");

        Amount = newAmount;
        AmountChanged?.Invoke(Amount);
    }
}