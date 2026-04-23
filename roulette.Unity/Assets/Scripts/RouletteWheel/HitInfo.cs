public class HitInfo
{
    public DozenType Dozen { get; set; }
    public string BallHash { get; set; }
    public string FieldColor { get; set; }
    public int FieldNumber { get; set; }
}

public enum DozenType
{
    Zero = 0,
    First = 1,
    Second = 2,
    Third = 3
}