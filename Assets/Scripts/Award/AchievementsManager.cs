using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    void Start()
    {
        Invoke("NewAward", 1);
    }

    private void NewAward()
    {
        if (GameManager.instance.awardPoints.Count > 0)
        {
            foreach (var data in GameManager.instance.awardPoints)
            {
                ProgressAchievement(data.Key, data.Value);
            }

            GameManager.instance.ClearAward();
        }
    }

    public void ProgressAchievement(string awardKey, int value)
    {
        if (AwardList.awardList.ContainsKey(awardKey))
        {
            AwardList.awardList[awardKey].currentValue += value;                                                   //???????? ?????????? ????

            PlayerPrefs.SetInt(AwardList.awardList[awardKey].awardName, AwardList.awardList[awardKey].currentValue);

            if (AchievementFloatingUI.instance != null && AwardList.awardList[awardKey].isAchieved == false)
            {
                AchievementFloatingUI.instance.ShowAchievementPopup(AwardList.awardList[awardKey]);
            }

            if (AwardList.awardList[awardKey].currentValue >= AwardList.awardList[awardKey].goalValue)
            {
                AwardList.awardList[awardKey].isAchieved = true;
            }
        }
    }
}
