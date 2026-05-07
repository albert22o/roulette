using TMPro;
using UnityEngine;

public class GoldenChipsLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    private GoldenChipsController goldenChipsController;

    private void Awake()
    {
        goldenChipsController = FindAnyObjectByType<GoldenChipsController>();
    }

    private void OnEnable()
    {
        goldenChipsController.CountChanged += UpdateText;
    }

    private void OnDisable()
    {
        goldenChipsController.CountChanged += UpdateText;
    }

    private void UpdateText(int count)
    {
        text.text = count.ToString();
    }
}
