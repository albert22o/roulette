using System;
using UnityEngine;
using TMPro;

public class ChipSlotView : MonoBehaviour
{
    public int SlotIndex { get; private set; }
    public GameObject ChipObject { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;

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

    public void DetachChip()
    {
        if (ChipObject == null) return;
        ChipObject.transform.SetParent(null);
    }

    public void ClearChip()
    {
        if (ChipObject == null) return;
        Destroy(ChipObject);
        ChipObject = null;

        if (nameText != null)
        {
            nameText.text = "";
        }
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

    public void SetChipName(string name)
    {
        if (nameText != null)
        {
            nameText.text = name;
        }
    }
}