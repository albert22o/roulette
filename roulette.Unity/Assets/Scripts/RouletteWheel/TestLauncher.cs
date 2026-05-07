using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TestLauncher : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private VisualComponent[] visualComponents;

    private bool isNeedReroll = false;


    private void OnEnable()
    {
        rouletteWheel.AllBallsStopped += OnAllBallsStopped;
    }

    private void OnDisable()
    {
        rouletteWheel.AllBallsStopped -= OnAllBallsStopped;
    }

    void Start()
    {
        StartCoroutine(PersistentLaunching());
    }

    public IEnumerator PersistentLaunching()
    {
        yield return new WaitForSeconds(delay);

        rouletteWheel.AddBall("1", visualComponents[0]);
    }

    private void OnAllBallsStopped(HitInfo[] hitsInfo)
    {
        foreach (var hit in hitsInfo)
        {
            Debug.Log($"Ball {hit.BallHash} stopped at field with number {hit.FieldNumber}, color {hit.FieldColor}, dozen {hit.Dozen}");
        }

        if (isNeedReroll == true)
        {
            rouletteWheel.Reroll("1", "2");
            isNeedReroll =false;
        }
    }

    private readonly int[] numbers = new int[]
    {
        0,
        32,15,19,4,21,2,25,17,34,6,
        27,13,36,11,30,8,23,10,5,24,
        16,33,1,20,14,31,9,22,18,29,
        7,28,12,35,3,26
    };

    private readonly string[] colors = new string[]
    {
        "Green",
        "Red","Black","Red","Black","Red","Black","Red","Black","Red","Black",
        "Red","Black","Red","Black","Red","Black","Red","Black","Red","Black",
        "Red","Black","Red","Black","Red","Black","Red","Black","Red","Black",
        "Red","Black","Red","Black","Red","Black"
    };

    private async void Awake()
    {
        if (rouletteWheel == null)
        {
            Debug.LogError("RouletteWheelLoader: RouletteWheel reference is missing!");
            return;
        }
        await Task.Delay(100);
        LoadFields();
    }

    private void LoadFields()
    {
        for (int i = 0; i < 37; i++)
        {
            rouletteWheel.SetFieldNumber(i, numbers[i]);
            rouletteWheel.SetFieldColor(i, colors[i]);
        }

        Debug.Log("Roulette fields loaded successfully.");
    }
}



