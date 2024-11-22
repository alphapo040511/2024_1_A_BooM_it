using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()
    {
        Vector3Int[] explosionRange = new Vector3Int[13];

        Vector3 referenceDirection = Vector3.right;
        Vector3 forwardDirection = transform.forward;

        float angle = Vector3.Angle(forwardDirection, referenceDirection);

        int temp = 0;
        for (int i = -6; i <= 6; i++)
        {
            if (angle <= 45 || angle >= 135f)
            {
                explosionRange[temp++] = new Vector3Int(0, 0, i);
            }
            else
            {
                explosionRange[temp++] = new Vector3Int(i, 0, 0);
            }
        }

        return explosionRange;
    }

    public override void KnockBack(float distance, float force)
    {
        base.KnockBack(2);
    }
}
