using System;
using UnityEngine;
using Zenject;

public class ShopSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buyClip;

    [SerializeField] private AudioClip chipBoughtClip;


    [Inject] private SignalBus signalBus;

    void Start()
    {
        signalBus.Subscribe<ItemPucharedSignal>(OnItemBought);
        signalBus.Subscribe<ChipBoughtSignal>(OnChipBought);
    }

    private void OnItemBought()
    {
        audioSource.PlayOneShot(buyClip);
    }

    private void OnChipBought()
    {
        audioSource.PlayOneShot(chipBoughtClip);
    }
}
