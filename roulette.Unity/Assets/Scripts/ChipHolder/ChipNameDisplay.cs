using UnityEngine;
using TMPro;

public class ChipNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetName(string name)
    {
        if (nameText != null)
        {
            nameText.text = name;
        }
    }
}