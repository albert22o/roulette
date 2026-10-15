using TMPro;
using UnityEngine;

[RequireComponent(typeof(WorldSpaceTooltip))]
public class SimpleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, mainText;
    private WorldSpaceTooltip worldSpaceTooltip;

    private void Start()
    {
        worldSpaceTooltip = GetComponent<WorldSpaceTooltip>();
    }

    public void Show(string title, string  mainText, Vector3 position)
    {
        this.title.text = title;
        this.mainText.text = mainText;
        worldSpaceTooltip.Show(position);
    }

    public void Hide()
    {
        worldSpaceTooltip.Hide();
    }
}
