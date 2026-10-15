using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetsGUIDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;

    public void SetRemainingBets(int remainingBets, int maxBets)
    {
        counter.text = $"{maxBets - remainingBets}/{maxBets}";
    }
}
