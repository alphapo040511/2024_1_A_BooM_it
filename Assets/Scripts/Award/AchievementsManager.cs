using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance { get; private set; }
    public Action<string> addAward;

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

    void Start()
    {
        if(GameManager.instance.awardPoints.Count > 0)
        {
            foreach(var data in GameManager.instance.awardPoints)
            {
                ProgressAchievement(data.Key, data.Value);
            }

            GameManager.instance.ClearAward();
        }
    }

    public void ProgressAchievement(string awardKey, int value)
    {
        Debug.Log(awardKey + "????");
        if(AwardList.awardList.ContainsKey(awardKey))
        {
            AwardList.awardList[awardKey].currentValue += value;                                                   //???????? ?????????? ????

            if (AchievementFloatingUI.instance != null && AwardList.awardList[awardKey].isAchieved == false)
            {
                AchievementFloatingUI.instance.ShowAchievementPopup(AwardList.awardList[awardKey]);
            }

            if (AwardList.awardList[awardKey].currentValue >= AwardList.awardList[awardKey].goalValue)
            {
                AwardList.awardList[awardKey].isAchieved = true;
            }

            addAward?.Invoke(awardKey);
        }
    }
}
