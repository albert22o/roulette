using UnityEngine;

[CreateAssetMenu(fileName = "LemonFoodData", menuName = "Food/LemonFoodData")]
public class LemonFoodData : FoodData
{
    public override Food CreateFood(FoodDependencies deps) => new LemonFood(deps.TableConfig);
}