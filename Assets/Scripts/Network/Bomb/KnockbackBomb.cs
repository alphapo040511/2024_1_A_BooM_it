using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()
    {
        return null;
    }

    public override void KnockBack(float distance, float force)
    {
        base.KnockBack(6, 20);
    }
}
