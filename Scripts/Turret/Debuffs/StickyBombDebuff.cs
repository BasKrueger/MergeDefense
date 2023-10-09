using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombDebuff : Debuff
{
    [SerializeField, Range(0,1)]
    private float slow;
    [SerializeField, AssetsOnly]
    private ParticleSystem explosion;

    protected override void OnUpdate(Enemy target)
    {
        base.OnUpdate(target);

        target.speed.percentBonus -= Time.deltaTime / maxDuration * slow;
    }

    protected override void Remove(Enemy target)
    {
        target.speed.percentBonus += slow;
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity).gameObject, 3);
        target.TakeDamage(base.damage, this);
        base.Remove(target);
    }
}
