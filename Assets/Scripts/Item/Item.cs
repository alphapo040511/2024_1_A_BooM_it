using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    SpeedUp,
    Shield,
}

public class Item : MonoBehaviour
{
    // 아이템 속성
    public int itemNumber;                     // 아이템 번호
    public ItemType itemType;                  // 아이템 타입
    public Texture2D itemTexture;                // 아이템 UI 텍스처
    public bool isUsable = true;               // 사용 가능한 상태
    public int maxUses = 1;                    // 사용 가능 횟수
    public float cooldownTime = 5f;            // 아이템 쿨타임 (초 단위)

    private int remainingUses;                 // 남은 사용 가능 횟수
    private bool isOnCooldown = false;         // 쿨타임 상태

    private void Start()
    {
        // 초기 남은 사용 횟수를 최대 사용 횟수로 설정
        remainingUses = maxUses;
        isOnCooldown = false;
    }

    // 아이템 사용 메서드
    public void UseItem(Player player)
    {
        // 아이템이 사용 가능하고 쿨타임 중이 아닐 경우에만 사용
        if (isUsable && !isOnCooldown && remainingUses > 0)
        {
            Debug.Log("Item used: " + itemNumber);

            // 아이템 사용 로직 추가 (예: 플레이어 체력 회복, 무기 발사 등)
            PerformItemAction(player);

            remainingUses--; // 사용 횟수 감소

            if (remainingUses <= 0)
            {
                isUsable = false; // 더 이상 사용 불가
            }
            else
            {
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

    private void SpeedUp(Player player)
    {
        player.UpdateSkillState(SkillState.SpeedUp);
    }

    private void Shield(Player player)
    {
        player.UpdateSkillState(SkillState.Resisting);
    }

    // 쿨타임을 관리하는 코루틴
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
