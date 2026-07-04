using System;
using System.Collections.Generic;

public class FoodHolder
{
    public event Action<Food> FoodApplied;

    private readonly List<Food> appliedHistory = new();
    private readonly FoodFactory foodFactory;

    public IReadOnlyList<Food> AppliedHistory => appliedHistory;

    public FoodHolder(FoodFactory foodFactory)
    {
        this.foodFactory = foodFactory;
    }

    public void AddFood(FoodData foodData)
    {
        var food = foodFactory.Create(foodData);
        food.Apply();

        appliedHistory.Add(food);

        FoodApplied?.Invoke(food);
    }
}