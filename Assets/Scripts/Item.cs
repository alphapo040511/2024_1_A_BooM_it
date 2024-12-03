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
    // ������ �Ӽ�
    public string itemIndex;                     // ������ �ε���
    public string itemName;                 //������ �̸�
    [TextArea(3,5)]public string Description;              //������ ����
    public ItemType itemType;                  // ������ Ÿ��
    public Sprite itemImage;                // ������ UI �ؽ�ó
    public NetworkPrefabRef bombPrefab;         //��ź ������
    public NetworkParabola bombParabola;
    public bool isUsable = true;               // ��� ������ ����
    public int maxUses = 1;                    // ��� ���� Ƚ��
    public float cooldownTime = 5f;            // ������ ��Ÿ�� (�� ����)
    public float duration = 5f;                 //������ ���ӽð�

    private int remainingUses;                 // ���� ��� ���� Ƚ��
    public bool isOnCooldown = false;         // ��Ÿ�� ����

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        // �ʱ� ���� ��� Ƚ���� �ִ� ��� Ƚ���� ����
        remainingUses = maxUses;
        isUsable = true;
        isOnCooldown = false;
    }

    // ������ ��� �޼���
    public void UseItem(Player player)
    {
        // �������� ��� �����ϰ� ��Ÿ�� ���� �ƴ� ��쿡�� ���
        if (isUsable && !isOnCooldown)
        {
            if (remainingUses > 0 || itemType == ItemType.Weapon || itemType == ItemType.TutorialBomb)
            {
                Debug.Log("Item used: " + itemIndex);

                // ������ ��� ���� �߰� (��: �÷��̾� ü�� ȸ��, ���� �߻� ��)
                PerformItemAction(player);

                remainingUses--; // ��� Ƚ�� ����

                if (remainingUses <= 0 && (itemType != ItemType.Weapon && itemType != ItemType.TutorialBomb))
                {
                    isUsable = false; // �� �̻� ��� �Ұ�
                }
                StartCoroutine(StartCooldown()); // ��Ÿ�� ����
            }
        }
        else
        {
            Debug.Log("Item is not usable right now.");
        }
    }

    public void UseItem(SinglePlayer player)
    {
        // �������� ��� �����ϰ� ��Ÿ�� ���� �ƴ� ��쿡�� ���
        if (isUsable && !isOnCooldown)
        {
            if (remainingUses > 0 || itemType == ItemType.Weapon || itemType == ItemType.TutorialBomb)
            {
                Debug.Log("Item used: " + itemIndex);

                // ������ ��� ���� �߰� (��: �÷��̾� ü�� ȸ��, ���� �߻� ��)
                PerformItemAction(player);

                remainingUses--; // ��� Ƚ�� ����

                if (remainingUses <= 0 && (itemType != ItemType.Weapon && itemType != ItemType.TutorialBomb))
                {
                    isUsable = false; // �� �̻� ��� �Ұ�
                }
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

    private void PerformItemAction(SinglePlayer player)
    {
        // ������ Ÿ�Կ� ���� Ư�� ��� ���� (��: ü�� ȸ��, �� Ȱ��ȭ ��)
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
        Debug.Log("Ʃ�丮�� ��ź �߻�");
    }

    private void TutoShield(SinglePlayer player)
    {
        Debug.Log("Ʃ�丮�� ���� ���");
    }

    // ��Ÿ���� �����ϴ� �ڷ�ƾ
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
