using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem hitEffect;
    [SerializeField]
    private bool hitFollowTarget;

    protected int damage;
    protected float speed;
    protected Vector3 targetPosition;
    protected Enemy targetEnemy;

    private bool impacted = false;

    public virtual void Setup(int damage, float speed, Vector3 targetPosition, Enemy targetEnemy)
    {
        this.damage = damage;
        this.speed = speed;
        this.targetPosition = targetPosition;
        this.targetEnemy = targetEnemy;

        Destroy(this.gameObject, 5);
    }

    private void Update()
    {
        Move(targetPosition, targetEnemy);
    }

    protected virtual void Move(Vector3 target, Enemy targetEnemy)
    {
        if (targetEnemy != null)
        {
            Vector3 targetVector = targetEnemy.transform.position;
            targetVector.y = transform.position.y;
            transform.LookAt(targetVector);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy hit = other.GetComponent<Enemy>();
        if(hit == null)
        {
            return;
        }

        if (!impacted)
        {
            impacted = true;
            Impact(hit);
        }
    }

    protected virtual void Impact(Enemy other)
    {
        if(other != null)
        {
            other.TakeDamage(damage, this);
            SpawnHitParticles(other.GetComponent<Enemy>());
        }

        Destroy(this.gameObject);
    }

    protected void SpawnHitParticles(Enemy enemy)
    {
        ParticleSystem active = Instantiate(hitEffect);
        active.gameObject.name = "Hit Particles";

        if (hitFollowTarget)
        {
            active.transform.SetParent(enemy.transform);
        }

        active.transform.position = transform.position + transform.forward * 0.001f;
        active.transform.LookAt(transform);
        active.Play();

        Destroy(active.gameObject, 1);
    }

    protected void SpawnHitParticles()
    {
        ParticleSystem active = Instantiate(hitEffect);
        active.gameObject.name = "Hit Particles";

        active.transform.position = transform.position + transform.forward * 0.001f;
        active.transform.LookAt(transform);
        active.Play();

        Destroy(active.gameObject, 1);
    }
}
