using UnityEngine;

[CreateAssetMenu(fileName = "CherryFoodData", menuName = "Food/CherryFoodData")]
public class CherryFoodData : FoodData
{
    public override Food CreateFood(FoodDependencies deps) => new CherryFood(deps.TableInfo);
}