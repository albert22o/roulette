using UnityEngine;

[CreateAssetMenu(fileName = "GeneralMultipilerCardData", menuName = "Cards/GeneralMultipilerCardData")]
public class GeneralMultipilerCardData : ItemData
{
    [SerializeField] private float multiplier;

    public float Multiplier => multiplier;
}
