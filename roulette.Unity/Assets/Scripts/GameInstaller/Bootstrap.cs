using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] BetTableView betTableView;
    void Awake()
    {
        betTableView.Init();
    }
}
