using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()       //�ϴ� z������ �������� �÷��̾��� ���� �������� �����ϱ�
    {
        Vector3Int[] explosionRange = new Vector3Int[9];
        int temp = 0;
        for (int z = -4; z <= 4; z++)
        {
            explosionRange[temp++] = new Vector3Int(0, 0, z);
        }

        return explosionRange;
    }
}
