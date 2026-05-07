using UnityEngine;

public class GoldenChipsTest : MonoBehaviour
{
    [SerializeField] private int initMoney;
    void Start()
    {
        FindAnyObjectByType<GoldenChipsController>().AddChips(initMoney);
    }
}
