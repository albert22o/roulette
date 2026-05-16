using UnityEngine;

public class RoundScopeView : MonoBehaviour
{
    [SerializeField] BetTableView betTableView;
    [SerializeField] RoundManagerView roundManagerView;
    [SerializeField] RoundStateManagerView changer;
    [SerializeField] BallResultView ballResultView;
    [SerializeField] RoundEntryZone roundEntryZone;

    public void Init()
    {
        roundManagerView.Init();
        betTableView.Init();
        changer.Init();
        roundEntryZone.Init();
        ballResultView.Init();
    }
}
