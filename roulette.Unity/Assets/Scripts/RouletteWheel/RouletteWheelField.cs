using System;

[System.Serializable]
public class RouletteWheelField
{
    private ColorType color;
    private int number;
    private DozenType dozen;

    public event Action<int> NumberChanged;
    public event Action<ColorType> ColorChanged;

    public ColorType Color
    {
        get => color;
        set { color = value; ColorChanged?.Invoke(color); }
    }

    public int Number
    {
        get => number;
        set { number = value; NumberChanged?.Invoke(number); }
    }

    public DozenType Dozen
    {
        get => dozen;
        set { dozen = value; }
    }
}

