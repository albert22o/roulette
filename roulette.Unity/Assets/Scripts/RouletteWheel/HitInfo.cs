public class HitInfo
{
    public DozenType Dozen { get; set; }
    /// <summary>
    /// Уникальный хеш шара, который остановился на поле
    /// </summary>
    public string BallHash { get; set; }
    public string FieldColor { get; set; }
    /// <summary>
    /// Номер поля, на котором остановился шар
    /// Может отличаться от фактической позиции поля
    /// </summary>
    public int FieldNumber { get; set; }
    /// <summary>
    /// Позиция поля, на котором остановился шар
    /// </summary>
    public int FieldPosition { get; set; }
}

public enum DozenType
{
    Zero = 0,
    First = 1,
    Second = 2,
    Third = 3
}