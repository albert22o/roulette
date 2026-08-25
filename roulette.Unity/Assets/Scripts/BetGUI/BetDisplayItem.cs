using UnityEngine;
using UnityEngine.UI;

public class BetDisplayItem : MonoBehaviour
{
    [SerializeField] private RawImage icon;

    [SerializeField] private Texture remainingBet;
    [SerializeField] private Texture closedBet;

    public void SetBet(bool isRemaining)
    {
        icon.texture = isRemaining ? remainingBet : closedBet;
    }
}
