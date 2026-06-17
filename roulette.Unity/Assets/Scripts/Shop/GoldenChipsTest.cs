using UnityEngine;
using Zenject;

public class GoldenChipsTest : MonoBehaviour
{
    [SerializeField] private int initMoney;
    private GoldenChips goldenChips;

    [Inject]
    public void Construct(GoldenChips goldenChips)
    {
        this.goldenChips = goldenChips;
    }

    void Start()
    {
        goldenChips.Add(initMoney);
    }
}
