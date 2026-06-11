using System;
[System.Serializable]
public class RouletteWheelField
{
    private string color;
    private int number;

    public event Action<int> NumberChanged;
    public event Action<string> ColorChanged;

    public string Color { 
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
}
