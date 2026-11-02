using TMPro;
using UnityEngine;

public class ChipSlotView : MonoBehaviour
{
    public int SlotIndex { get; private set; }
    public GameObject ChipObject { get; private set; }
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private Animator animator;

    private readonly int showMultiplierTrigger = Animator.StringToHash("ShowMultiplier");
    private Vector3 worldPosition;

    public void Init(int slotIndex, Vector3 position)
    {
        SlotIndex = slotIndex;
        worldPosition = position;
        transform.position = position;
    }

    public void PlaceChip(GameObject chipObject)
    {
        ChipObject = chipObject;
        chipObject.transform.SetParent(transform);
        chipObject.transform.position = worldPosition;
    }

    public GameObject DetachChip()
    {
        if (ChipObject == null)
            return null;

        var chip = ChipObject;
        chip.transform.SetParent(null);
        ChipObject = null; // обнуляем ссылку, чтобы слот не считался занятым во время драга

        return chip;
    }

    public void ClearChip()
    {
        if (ChipObject == null) return;

        Destroy(ChipObject);
        ChipObject = null;
    }

    /// <summary>
    /// Обнуляет ссылку на фишку в этом слоте, НЕ уничтожая сам GameObject.
    /// Используется, когда фишка физически переезжает в другой слот
    /// (свап, сдвиг при удалении слота) — в отличие от ClearChip(),
    /// который предназначен для случая, когда фишка реально удаляется
    /// из игры (Destroy).
    /// </summary>
    public void ClearReference()
    {
        ChipObject = null;
    }

    public Vector3 GetWorldPosition() => worldPosition;

    public void RestoreChip(GameObject chipObject)
    {
        ChipObject = chipObject;
        chipObject.transform.SetParent(transform);
    }

    public void PlayAnimation()
    {
        ChipObject?.GetComponent<Animator>().SetTrigger("start");
    }

    public void ShowMultiplier(float value, OperationType operation)
    {
        if (value != 0)
        {
            string sign = "";
            switch (operation)
            {
                case OperationType.Add:
                    sign = "+";
                    break;
                case OperationType.Multiply:
                    sign = "×";
                    break;
            }
               
            multiplierText.text = sign + value.ToString();
            animator.SetTrigger(showMultiplierTrigger);
        }
    }

    public void SetChipName(string name)
    {
    }
}