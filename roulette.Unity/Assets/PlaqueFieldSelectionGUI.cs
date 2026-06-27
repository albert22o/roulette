using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlaqueFieldSelectionGUI : MonoBehaviour
{
    private PlaqueFieldsSelector plaqueFieldsSelector;


    [SerializeField] private TextMeshProUGUI selectionDescription;
    [SerializeField] private Button applyButton;

    [Inject] 
    public void Construct(PlaqueFieldsSelector selector)
    {
        this.plaqueFieldsSelector = selector;
    }

    private void OnEnable()
    {
        plaqueFieldsSelector.SelectionRequestChanged += OnSelectionRequestChanged;
        plaqueFieldsSelector.IsSelectionValidChanged += OnSelectionValidChanged;
        applyButton.onClick.AddListener(Apply);
    }

    private void OnDisable()
    {
        plaqueFieldsSelector.SelectionRequestChanged -= OnSelectionRequestChanged;
        plaqueFieldsSelector.IsSelectionValidChanged -= OnSelectionValidChanged;
        applyButton.onClick.RemoveListener(Apply);
    }

    public void Apply()
    {
        plaqueFieldsSelector.ApplySelection();
    }

    private void OnSelectionRequestChanged(SelectionRequest request)
    {
        selectionDescription.text = request.Hint;
    }

    public void OnSelectionValidChanged(bool isValid)
    {
        applyButton.interactable = isValid;
    }
}
