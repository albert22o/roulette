using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentBetGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private Image image;
    [SerializeField]
    private SerializedDictionary<ColorType, Sprite> sprites = new();
    [SerializeField] private TextMeshProUGUI betSum;

    public void SetFieldData(int number, ColorType color, int betSum)
    {
        this.number.text = number.ToString();
        image.sprite = sprites[color];
        this.betSum.text = betSum.ToString();
    }
}
