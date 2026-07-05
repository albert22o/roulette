public class HitInfo
{
    public DozenType Dozen { get; set; }
    /// <summary>
    /// Уникальный хеш шара, который остановился на поле
    /// </summary>
    public string BallHash { get; set; }
    public ColorType FieldColor { get; set; }
    /// <summary>
    /// Номер поля, на котором остановился шар
    /// Может отличаться от фактической позиции поля
    /// </summary>
    public int FieldNumber { get; set; }
    public int FieldIndex { get; set; }
}

public enum DozenType
{
    Zero = 0,
    First = 1,
    Second = 2,
    Third = 3
}
public enum ColorType
{
    Red,
    Black,
    Green,
    White,
    Grey
}

[System.Flags]
public enum ColorMask
{
    None = 0,
    Red = 1 << 0,
    Black = 1 << 1,
    Green = 1 << 2,
}

