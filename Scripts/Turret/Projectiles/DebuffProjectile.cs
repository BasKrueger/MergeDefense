using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffProjectile : Projectile
{
    [SerializeField, AssetsOnly]
    private Debuff debuff;

    protected override void Impact(Enemy other)
    {
        Debuff active = Instantiate(debuff);
        active.transform.position = transform.position;
        active.transform.SetParent(other.transform);

        active.SetUp(other, base.damage);
        base.damage = 0;

        base.Impact(other);
    }
}
