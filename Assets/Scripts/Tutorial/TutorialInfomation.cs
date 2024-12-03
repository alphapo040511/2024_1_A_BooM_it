using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TutorialInfomation : MonoBehaviour
{
    [TextArea]public string infomation;
    public TextMeshProUGUI infoText;

    public void Start()
    {
        UpdateData(0, 0);
    }

    public void UpdateData(int value, int maxValue)
    {
        string temp = infomation.Replace("Value", $"{value}");
        temp = temp.Replace("Max", $"{maxValue}");
        infoText.text = temp;
    }
}
