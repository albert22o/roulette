using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class ShopSound : MonoBehaviour
{
    [SerializeField] private float soundCooldown = 0.1f;
    [Header("Purchase")]
    [SerializeField] private AudioSource buyAudioSource;
    [SerializeField] private AudioClip itemPurchasedClip;

    [Header("Chip Places")]
    [SerializeField] private AudioSource chipPlaceAudioSource;
    [SerializeField] private ChipPlaceAnimationController[] chipPlaces;
    [SerializeField] private AudioClip chipPlaceMoveClip;

    [Header("Food Cap")]
    [SerializeField] private AudioSource foodCapRiseSource;
    [SerializeField] private ShopFoodPlaceAnimationController foodCap;
    [SerializeField] private AudioClip foodCapRiseClip;

    [Header("Counters")]
    [SerializeField] private DrumNumberWheel[] shopDrumWheels;
    [SerializeField] private AudioClip drumWheelStepClip;
    [SerializeField] private AudioSource DrumWheelTickSource;
    private readonly Dictionary<AudioClip, float> _lastPlayTimes = new Dictionary<AudioClip, float>();

    private Shop shop;
    private GameStateManager gameStateManager;
    private bool isSoundEnabled;


    [Inject]
    private void Construct(Shop shop, GameStateManager gameStateManager)
    {
        this.shop = shop;
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        shop.ItemPurchased += OnItemPurchased;
        foreach (var chipPlace in chipPlaces)
        {
            chipPlace.MovingStarted += OnChipPlaceMovingStarted;
        }
        foreach (var wheel in shopDrumWheels)
        {
            wheel.WheelStepPassed += OnWheelStepPassed;
        }
        foodCap.CapRised += OnFoodCapRise;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        isSoundEnabled = state == GameStateManager.GameState.Shopping;
    }

    private void OnDestroy()
    {
        if (shop != null)
            shop.ItemPurchased -= OnItemPurchased;

        if (chipPlaces != null)
        {
            foreach (var chipPlace in chipPlaces)
            {
                if (chipPlace != null)
                    chipPlace.MovingStarted -= OnChipPlaceMovingStarted;
            }
        }

        if (foodCap != null)
            foodCap.CapRised -= OnFoodCapRise;

        foreach (var wheel in shopDrumWheels)
        {
            if (wheel != null)
            {
                wheel.WheelStepPassed -= OnWheelStepPassed;
            }
        }

        if (gameStateManager != null)
            gameStateManager.GameStateChanged -= OnGameStateChanged;
    }
    private void OnFoodCapRise()
    {
        PlaySound(foodCapRiseSource, foodCapRiseClip);
    }

    private void OnChipPlaceMovingStarted()
    {
        PlaySound(chipPlaceAudioSource, chipPlaceMoveClip);
    }

    private void OnItemPurchased()
    {
        PlaySound(buyAudioSource, itemPurchasedClip);
    }
    private void OnWheelStepPassed()
    {
        PlaySound(DrumWheelTickSource, drumWheelStepClip);
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (isSoundEnabled == false) return;
        if (source == null || clip == null) return;

        if (_lastPlayTimes.TryGetValue(clip, out float lastTime))
        {
            if (Time.time - lastTime < soundCooldown)
            {
                return;
            }
        }
        source.PlayOneShot(clip);
        _lastPlayTimes[clip] = Time.time;
    }
}
