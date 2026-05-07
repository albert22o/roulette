using System;
using System.Threading.Tasks;

public class RoundStateManager
{
    public event Action<BetState> StateChanged;

    public RoundStateManager()
    {
        Test();
    }

    private async Task Test()
    {
        await Task.Delay(100);
        ChangeState(BetState.PlacingBets);
    }

    public void ChangeState(BetState state)
    {
        StateChanged?.Invoke(state);
    }

    public enum BetState
    {
        PlacingBets,
        WheelSpining,
        ChoosingReward,
        NotActive
    }
}