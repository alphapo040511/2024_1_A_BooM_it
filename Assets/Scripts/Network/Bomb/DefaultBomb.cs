using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DefaultBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()
    {
        Vector3Int[] explosionRange = new Vector3Int[13];
        int temp = 0;
        for(int x = -2; x <=2; x++)
        {
            for(int z = -2; z <=2; z++)
            {
                if(Mathf.Abs(x) + Mathf.Abs(z) <= 2)
                explosionRange[temp++] = new Vector3Int(x,0,z);
            }
        }

        return explosionRange;
    }
}
