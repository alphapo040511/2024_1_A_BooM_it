using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public TutorialInfomation info;
    private bool isClear = false;
    private TutorialType type;
    private int goalValue;
    private int currentValue;

    private void Awake()
    {
        instance = this;
    }

    public void Init(TutorialType type, int value)
    {
        this.type = type;
        goalValue = value;
        info.UpdateData(0, goalValue);
    }

    public void AddPoint(TutorialType type)
    {
        if (this.type != type) return;

        currentValue++;

        info.UpdateData(currentValue, goalValue);

        if(currentValue >= goalValue)
        {
            StageClear();
        }
    }

    public void ResetPoint()
    {
        currentValue = 0;
        info.UpdateData(currentValue, goalValue);
    }

    public void StageClear()
    {
        if (isClear) return;
        isClear = true;
        Debug.Log("스테이지 클리어");
    }
}
