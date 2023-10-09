using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff : SerializedMonoBehaviour
{
    protected bool unlimited = false;
    [SerializeField]
    protected float maxDuration { get; private set; }
    protected int damage = 0;

    protected float activeDuration { get; private set; }

    private Enemy target;


    public virtual void SetUp(Enemy enemy, int damage)
    {
        this.damage = damage;
        target = enemy;

        enemy.OnDeath.AddListener(OnDeath);
        enemy.OnTakeDamage.AddListener(OnTakeDamage);

        activeDuration = maxDuration;
    }

    protected virtual void OnDeath(Enemy died)
    {
        Remove(died);
    }

    protected virtual void OnTakeDamage(Enemy damaged, int damage, Projectile source)
    {

    }

    protected virtual void OnUpdate(Enemy target)
    {

    }

    private void Update()
    {
        OnUpdate(target);

        activeDuration -= Time.deltaTime;
        if (activeDuration < 0 && !unlimited)
        {
            Remove(target);
        }
    }

    protected virtual void Remove(Enemy target)
    {
        Destroy(this.gameObject);
    }
}
