using Fusion;
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
    // ������ �Ӽ�
    public string itemNumber;                     // ������ �̸�
    public string Description;              //������ ����
    public ItemType itemType;                  // ������ Ÿ��
    public Sprite itemImage;                // ������ UI �ؽ�ó
    public NetworkPrefabRef bombPrefab;         //��ź ������
    public NetworkParabola bombParabola;
    public bool isUsable = true;               // ��� ������ ����
    public int maxUses = 1;                    // ��� ���� Ƚ��
    public float cooldownTime = 5f;            // ������ ��Ÿ�� (�� ����)

    private int remainingUses;                 // ���� ��� ���� Ƚ��
    private bool isOnCooldown = false;         // ��Ÿ�� ����

    private void Start()
    {
        // �ʱ� ���� ��� Ƚ���� �ִ� ��� Ƚ���� ����
        remainingUses = maxUses;
        isOnCooldown = false;
    }

    // ������ ��� �޼���
    public void UseItem(Player player)
    {
        // �������� ��� �����ϰ� ��Ÿ�� ���� �ƴ� ��쿡�� ���
        if (isUsable && !isOnCooldown)
        {
            if (remainingUses > 0 || itemType == ItemType.Weapon)
            {
                Debug.Log("Item used: " + itemNumber);

                // ������ ��� ���� �߰� (��: �÷��̾� ü�� ȸ��, ���� �߻� ��)
                PerformItemAction(player);

                remainingUses--; // ��� Ƚ�� ����

                if (remainingUses <= 0 && itemType != ItemType.Weapon)
                {
                    isUsable = false; // �� �̻� ��� �Ұ�
                }
                else
                {
                    StartCoroutine(StartCooldown()); // ��Ÿ�� ����
                }
            }
        }
        else
        {
            Debug.Log("Item is not usable right now.");
        }
    }

    // ���� ������ ����� �����ϴ� �޼���
    private void PerformItemAction(Player player)
    {
        // ������ Ÿ�Կ� ���� Ư�� ��� ���� (��: ü�� ȸ��, �� Ȱ��ȭ ��)
        switch (itemType)
        {
            case ItemType.Weapon:
                //���� ���
                Fire(player);
                break;
            case ItemType.SpeedUp:
                // �̵��ӵ� ����
                SpeedUp(player);
                break;
            case ItemType.Shield:
                // �� ��� �߰�
                Shield(player);
                break;
        }
    }

    private void Fire(Player player)
    {
        player.FirePosition(bombPrefab);
    }

    private void SpeedUp(Player player)
    {
        player.UpdateSkillState(SkillState.SpeedUp);
    }

    private void Shield(Player player)
    {
        player.UpdateSkillState(SkillState.Resisting);
    }

    // ��Ÿ���� �����ϴ� �ڷ�ƾ
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
