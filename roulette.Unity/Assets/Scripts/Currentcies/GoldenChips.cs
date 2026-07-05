public class GoldenChips : CurrencyResource
{
    public GoldenChips(CurrenciesConfig currenciesConfig)
    {
        Add(currenciesConfig.StartGoldenChips);
    }
}