using System;
using UnityEngine;

public class ChipSlotView : MonoBehaviour
{
    public int SlotIndex { get; private set; }
    public GameObject ChipObject { get; private set; }

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
}
