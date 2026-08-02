using System;
[System.Serializable]
public class RouletteWheelField
{
    private ColorType color;
    private int number;
    public bool isHighlighted;

    public event Action<int> NumberChanged;
    public event Action<ColorType> ColorChanged;
    public event Action<bool> HighlightChanged;

    public ColorType Color { 
        get => color;
        set
        {
            color = value;
            ColorChanged?.Invoke(color);
        }   
    }

    public int Number { get => number; set 
        {
            number = value;
            NumberChanged?.Invoke(number);
        }
    }

    public bool IsHighlighted
    {
        get => isHighlighted;
        set
        {
            if (isHighlighted != value)
            {
                isHighlighted = value;
                HighlightChanged?.Invoke(isHighlighted);
            }
        }
    }
}
