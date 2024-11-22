using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()
    {
        Vector3Int[] explosionRange = new Vector3Int[13];
        int temp = 0;
        for (int x = -3; x <= 3; x++)
        {
            for (int z = -3; z <= 3; z++)
            {
                if (x == 0 || z == 0)
                    explosionRange[temp++] = new Vector3Int(x, 0, z);
            }
        }

        return explosionRange;
    }
}
