using UnityEngine;

public class RouletteVisual : MonoBehaviour
{
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private Transform innerWheel;




    private void Update()
    {
        innerWheel.transform.rotation = rouletteWheel.InnerWheel.rotation;
    }
}
