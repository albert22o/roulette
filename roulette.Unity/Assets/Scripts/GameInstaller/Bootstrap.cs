using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] BetTableView betTableView;
    [SerializeField] RoundManagerView roundManagerView;
    void Awake()
    {
        roundManagerView.Init();
        betTableView.Init();

    }
}
