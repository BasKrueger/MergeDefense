using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [SerializeField]
    private float explosionRange = 1f;

    Vector3 startPosition;

    private float travelTime = 0f;
    private float activeTravelTime = 0f;

    public override void Setup(int damage, float speed, Vector3 targetPosition, Enemy targetEnemy)
    {
        startPosition = transform.position;
        targetPosition.y = startPosition.y;
        travelTime = Vector3.Distance(transform.position, targetPosition) / speed;
        activeTravelTime = 0;

        base.Setup(damage, speed, targetPosition, targetEnemy);
    }

    protected override void Impact(Enemy other)
    {
        foreach(Enemy enemy in FindObjectsOfType<Enemy>())
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < explosionRange && enemy != other)
            {
                enemy.TakeDamage(base.damage,this);
            }
        }

        base.Impact(other);
    }

    protected override void Move(Vector3 targetPosition, Enemy targetEnemy)
    {
        if(travelTime <= 0)
        {
            return;
        }

        activeTravelTime += Time.deltaTime;
        transform.position = cubeBezier3(startPosition, targetPosition, activeTravelTime / travelTime);

        if(activeTravelTime / travelTime > 1)
        {
            SpawnHitParticles();
            Impact(null);
        }
    }

    public Vector3 cubeBezier3(Vector3 p0, Vector3 p3, float t)
    {
        Vector3 p1 = (p3 - p0 / 2) * 0.25f + p0 + new Vector3(0, 0.5f, 0);
        Vector3 p2 = (p3 - p0 / 2) * 0.25f + p0 + new Vector3(0, 0.5f, 0);

        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;
        return f0 * p0 + f1 * p1 + f2 * p2 + f3 * p3;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
