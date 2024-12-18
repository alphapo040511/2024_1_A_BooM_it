using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    SpeedUp,
    Shield,
    TutorialBomb,
    TutorialShield,
}

public class Item : MonoBehaviour
{
    // 아이템 속성
    public string itemIndex;                     // 아이템 인덱스
    public string itemName;                 //아이템 이름
    [TextArea(3,5)]public string Description;              //아이템 설명
    public ItemType itemType;                  // 아이템 타입
    public Sprite itemImage;                // 아이템 UI 텍스처
    public NetworkPrefabRef bombPrefab;         //폭탄 프리팹
    public NetworkParabola bombParabola;
    public bool isUsable = true;               // 사용 가능한 상태
    public int maxUses = 1;                    // 사용 가능 횟수
    public float cooldownTime = 5f;            // 아이템 쿨타임 (초 단위)
    public float duration = 5f;                 //아이템 지속시간

    private int remainingUses;                 // 남은 사용 가능 횟수
    public bool isOnCooldown = false;         // 쿨타임 상태

    private void Start()
    {
        ItemReset();
    }

    public void ItemReset()
    {
        // 초기 남은 사용 횟수를 최대 사용 횟수로 설정
        remainingUses = maxUses;
        isUsable = true;
        isOnCooldown = false;
    }

    // 아이템 사용 메서드
    public void UseItem(Player player)
    {
        // 아이템이 사용 가능하고 쿨타임 중이 아닐 경우에만 사용
        if (isUsable && !isOnCooldown)
        {
            if (remainingUses > 0 || itemType == ItemType.Weapon || itemType == ItemType.TutorialBomb)
            {
                Debug.Log("Item used: " + itemIndex);

                // 아이템 사용 로직 추가 (예: 플레이어 체력 회복, 무기 발사 등)
                PerformItemAction(player);

                remainingUses--; // 사용 횟수 감소

                if (remainingUses <= 0 && (itemType != ItemType.Weapon && itemType != ItemType.TutorialBomb))
                {
                    isUsable = false; // 더 이상 사용 불가
                }
                StartCoroutine(StartCooldown()); // 쿨타임 시작
            }
        }
        else
        {
            Debug.Log("Item is not usable right now.");
        }
    }

    public void UseItem(SinglePlayer player)
    {
        // 아이템이 사용 가능하고 쿨타임 중이 아닐 경우에만 사용
        if (isUsable && !isOnCooldown)
        {
            if (remainingUses > 0 || itemType == ItemType.Weapon || itemType == ItemType.TutorialBomb)
            {
                Debug.Log("Item used: " + itemIndex);

                // 아이템 사용 로직 추가 (예: 플레이어 체력 회복, 무기 발사 등)
                PerformItemAction(player);

                remainingUses--; // 사용 횟수 감소

                if (remainingUses <= 0 && (itemType != ItemType.Weapon && itemType != ItemType.TutorialBomb))
                {
                    isUsable = false; // 더 이상 사용 불가
                }
                StartCoroutine(StartCooldown()); // 쿨타임 시작
            }
        }
        else
        {
            Debug.Log("Item is not usable right now.");
        }
    }

    // 실제 아이템 기능을 수행하는 메서드
    private void PerformItemAction(Player player)
    {
        // 아이템 타입에 따른 특정 기능 구현 (예: 체력 회복, 방어막 활성화 등)
        switch (itemType)
        {
            case ItemType.Weapon:
                //무기 사용
                Fire(player);
                break;
            case ItemType.SpeedUp:
                // 이동속도 증가
                SpeedUp(player);
                break;
            case ItemType.Shield:
                // 방어막 기능 추가
                Shield(player);
                break;
        }
    }

    private void PerformItemAction(SinglePlayer player)
    {
        // 아이템 타입에 따른 특정 기능 구현 (예: 체력 회복, 방어막 활성화 등)
        switch (itemType)
        {
            case ItemType.TutorialBomb:
                TutoFire(player);
                break;
            case ItemType.TutorialShield:
                TutoShield(player);
                break;
        }
    }

    private void Fire(Player player)
    {
        player.FirePosition(bombPrefab);
    }

    private void SpeedUp(Player player)
    {
        player.RPC_UpdateSkillState(SkillState.SpeedUp);
    }

    private void Shield(Player player)
    {
        player.RPC_UpdateSkillState(SkillState.Resisting);
    }

    private void TutoFire(SinglePlayer player)
    {
        Debug.Log("튜토리얼 폭탄 발사");
    }

    private void TutoShield(SinglePlayer player)
    {
        Debug.Log("튜토리얼 쉴드 사용");
    }

    // 쿨타임을 관리하는 코루틴
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
