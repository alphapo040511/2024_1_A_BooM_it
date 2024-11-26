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

    private void Update()
    {

    }

    public void ProgressAchievement(string awardKey, int value)
    {
        Debug.Log(awardKey + "달성");
        if(AwardList.awardList.ContainsKey(awardKey))
        {
            AwardList.awardList[awardKey].currentValue += value;                                                   //최대치를 넘기더라도 저장

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
