using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class ShopSound : MonoBehaviour
{
    [SerializeField] private ChipPlaceAnimationController[] chipPlaces;

    [SerializeField] private ShopFoodPlaceAnimationController foodCap;
    [SerializeField] private DrumNumberWheel[] shopDrumWheels;

    private Shop shop;
    private GameStateManager gameStateManager;
    private bool isSoundEnabled;
    private SFXManager soundManager;


    [Inject]
    private void Construct(Shop shop, GameStateManager gameStateManager, SFXManager soundManager)
    {
        this.shop = shop;
        this.gameStateManager = gameStateManager;
        this.soundManager = soundManager;
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
        if (isSoundEnabled == false) return;
        soundManager.Play("FoodCapRise");
    }

    private void OnChipPlaceMovingStarted()
    {
        if (isSoundEnabled == false) return;
        soundManager.Play("ChipPlaceMove");
    }

    private void OnItemPurchased()
    {
        if (isSoundEnabled == false) return;
        soundManager.Play("Purchase");
    }
    private void OnWheelStepPassed()
    {
        if (isSoundEnabled == false) return;
        soundManager.Play("CounterClick");
    }
}
