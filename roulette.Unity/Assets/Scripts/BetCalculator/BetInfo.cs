public class BetInfo
{
    public BetInfo(int fieldNumber, DozenType dozen, string fieldColor)
    {
        FieldNumber = fieldNumber;
        Dozen = dozen;
        FieldColor = fieldColor;
    }

    /// <summary>
    /// Номер поля, на котором остановился шар
    /// Может отличаться от фактической позиции поля
    /// </summary>
    public int FieldNumber { get; set; }
    public DozenType Dozen { get; set; }
    public string FieldColor { get; set; }
}