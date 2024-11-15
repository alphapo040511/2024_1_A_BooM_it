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
    public int itemNumber;                     // ������ ��ȣ
    public ItemType itemType;                  // ������ Ÿ��
    public Texture2D itemTexture;                // ������ UI �ؽ�ó
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
        if (isUsable && !isOnCooldown && remainingUses > 0)
        {
            Debug.Log("Item used: " + itemNumber);

            // ������ ��� ���� �߰� (��: �÷��̾� ü�� ȸ��, ���� �߻� ��)
            PerformItemAction(player);

            remainingUses--; // ��� Ƚ�� ����

            if (remainingUses <= 0)
            {
                isUsable = false; // �� �̻� ��� �Ұ�
            }
            else
            {
                StartCoroutine(StartCooldown()); // ��Ÿ�� ����
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
