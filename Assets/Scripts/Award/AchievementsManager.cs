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

    public void ProgressAchievement(string awardName, int value)
    {
        Debug.Log(awardName + "달성");
        if(AwardList.awardList.ContainsKey(awardName))
        {
            AwardList.awardList[awardName].currentValue += value;                                                   //최대치를 넘기더라도 저장
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
