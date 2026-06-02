using System;



public interface ICurrencyResource
{
    event Action<int> AmountChanged;
    int Amount { get; }
}
