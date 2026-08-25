using TMPro;
using UnityEngine;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    public void SetCount(float count)
    {
        tmp.SetText(count.ToString());
    }
}
