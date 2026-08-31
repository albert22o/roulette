using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GrayTestHelper : MonoBehaviour
{
    private TableInfo tableInfo;

    [Inject]
    public void Construct(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.gKey.wasPressedThisFrame)
        {
            MakeAllFieldsGray();
        }
    }

    private void MakeAllFieldsGray()
    {
        for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
        {
            tableInfo.SetPocketColor(i, ColorType.Grey);
        }
    }
}