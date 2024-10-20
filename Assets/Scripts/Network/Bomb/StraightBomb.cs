using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()       //일단 z축으로 터지지만 플레이어의 시점 기준으로 변경하기
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
