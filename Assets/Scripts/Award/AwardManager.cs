using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardManager : MonoBehaviour
{
    public static AwardManager instance { get; private set; }

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
        if(AwardList.awardList.ContainsKey(awardName))
        {
            AwardList.awardList[awardName].currentValue += value;
            if(AwardList.awardList[awardName].currentValue >= AwardList.awardList[awardName].goalValue)
            {
                AwardList.awardList[awardName].isAchieved = true;
            }

            //업적 관련 팝업 UI 기능 추가
        }
    }
}
