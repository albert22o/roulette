using UnityEngine;

[CreateAssetMenu(fileName = "GeneralMultipilerCardData", menuName = "Cards/GeneralMultipilerCardData")]
public class GeneralMultipilerCardData : ItemData
{
    [SerializeField] private int multiplier;

    public int Multiplier => multiplier;
}
