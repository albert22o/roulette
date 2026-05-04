using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    private Button button;
    
    public void Init(string title, Action callBack)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => callBack?.Invoke());

        if (title == null)
            Debug.LogError("Title is not assigned in ChoiceButton.");
        this.title.text = title;
    }
}