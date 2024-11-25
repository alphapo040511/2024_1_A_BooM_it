using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProgressAchievement("Kill", 1);
        }
    }

    public void ProgressAchievement(string awardName, int value)
    {
        Debug.Log(awardName + "�޼�");
        if(AwardList.awardList.ContainsKey(awardName))
        {
            AwardList.awardList[awardName].currentValue += value;                                                   //�ִ�ġ�� �ѱ���� ����
            if(AwardList.awardList[awardName].currentValue >= AwardList.awardList[awardName].goalValue)
            {
                AwardList.awardList[awardName].isAchieved = true;
            }

            if (AchievementFloatingUI.instance != null && AwardList.awardList[awardName].isAchieved == false)
            {
                AchievementFloatingUI.instance.ShowAchievementPopup(awardName, AwardList.awardList[awardName].currentValue, AwardList.awardList[awardName].goalValue);
            }
        }
    }
}
