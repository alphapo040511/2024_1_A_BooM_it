using System.Collections.Generic;

public class AwardList
{
    public static Dictionary<string, AwardData> awardList = new Dictionary<string, AwardData>
    {
        { 
            "Kill",
            new AwardData
            {
                awardName = "학살자",
                awardDescription = "멀티플레이에서 상대방을 100 회 처치하기",
                isAchieved = false,
                goalValue = 100,
                currentValue = 0,
            }
        },
        {
            "Win",
            new AwardData
            {
                awardName = "승리자",
                awardDescription = "멀티플레이에서 10 회 승리하기",
                isAchieved = false,
                goalValue = 10,
                currentValue = 0,
            }
        },
        {
            "Marathoner",
            new AwardData
            {
                awardName = "마라토너",
                awardDescription = "총합 10000 블록 만큼 이동하기",
                isAchieved = false,
                goalValue = 10000,
                currentValue = 0,
            }
        },
        {
            "WeaponMaster",
            new AwardData
            {
                awardName = "폭탄 마스터",
                awardDescription = "무기 종류 5개 이상 획득하기",
                isAchieved = false,
                goalValue = 5,
                currentValue = 0,
            }
        }
    };
}
