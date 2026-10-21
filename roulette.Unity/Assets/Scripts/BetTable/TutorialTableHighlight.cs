using System;

public class TutorialTableHighlight
{
    public event Action BlinkAllNumbersRequested;
    public event Action<int> BlinkBetRequested;   // индекс ячейки
    public event Action StopRequested;

    public void BlinkAllNumbers() => BlinkAllNumbersRequested?.Invoke();
    public void BlinkBet(int pocketIndex) => BlinkBetRequested?.Invoke(pocketIndex);
    public void Stop() => StopRequested?.Invoke();
}