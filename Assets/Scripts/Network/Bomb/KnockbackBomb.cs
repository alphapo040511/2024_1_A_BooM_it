using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnockbackBomb : NetworkParabola
{
    public override Vector3Int[] ExplosionRange()
    {
        return null;
    }

    public override void KnockBack(float distance, float force)
    {
        if (GameManager.instance.mapIndex.ToString().Split('_').Contains("Ice"))
        {
            base.KnockBack(6, 10);
        }
        else
        {
            base.KnockBack(6, 20);
        }
    }
}
