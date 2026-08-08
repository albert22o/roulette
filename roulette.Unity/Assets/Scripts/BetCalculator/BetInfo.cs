public class BetInfo
{
    public BetInfo(int fieldNumber, DozenType dozen, ColorType fieldColor, int index)
    {
        FieldNumber = fieldNumber;
        Dozen = dozen;
        FieldColor = fieldColor;
        Index = index;
    }

    /// <summary>
    /// Номер поля, на котором остановился шар
    /// Может отличаться от фактической позиции поля
    /// </summary>
    public int FieldNumber { get; set; }
    public DozenType Dozen { get; set; }
    public ColorType FieldColor { get; set; }
    public int Index { get; private set; }
}