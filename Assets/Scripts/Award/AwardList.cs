using System.Collections.Generic;

public class AwardList
{
    public static Dictionary<string, AwardData> awardList = new Dictionary<string, AwardData>
    {
        { 
            "Kill",
            new AwardData
            {
                awardName = "�л���",
                awardDescription = "��Ƽ�÷��̿��� ������ 100 ȸ óġ�ϱ�",
                isAchieved = false,
                goalValue = 100,
                currentValue = 0,
            }
        },
        {
            "Win",
            new AwardData
            {
                awardName = "�¸���",
                awardDescription = "��Ƽ�÷��̿��� 10 ȸ �¸��ϱ�",
                isAchieved = false,
                goalValue = 10,
                currentValue = 0,
            }
        },
        {
            "Marathoner",
            new AwardData
            {
                awardName = "�������",
                awardDescription = "���� 10000 ��� ��ŭ �̵��ϱ�",
                isAchieved = false,
                goalValue = 10000,
                currentValue = 0,
            }
        },
        {
            "WeaponMaster",
            new AwardData
            {
                awardName = "��ź ������",
                awardDescription = "���� ���� 5�� �̻� ȹ���ϱ�",
                isAchieved = false,
                goalValue = 5,
                currentValue = 0,
            }
        }
    };
}
