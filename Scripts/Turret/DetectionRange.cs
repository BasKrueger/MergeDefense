using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    [EnumToggleButtons]
    public detectionMode mode;

    private List<Enemy> reachableEnemies = new List<Enemy>();

    public void SetUp(float range)
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = range / 2;
        col.isTrigger = true;

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>() == null)
        {
            return;
        }

        other.GetComponent<Enemy>().OnDeath.AddListener(RemoveFromList);
        reachableEnemies.Add(other.GetComponent<Enemy>());
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Enemy>() == null)
        {
            return;
        }

        other.GetComponent<Enemy>().OnDeath.RemoveListener(RemoveFromList);
        RemoveFromList(other.GetComponent<Enemy>());
    }

    private void RemoveFromList(Enemy enemy)
    {
        reachableEnemies.Remove(enemy);
    }

    public Enemy GetEnemy(int customDetection = -1)
    {
        detectionMode toCheck = mode;

        if(customDetection != -1)
        {
            toCheck = (detectionMode)customDetection;
        }

        switch (toCheck)
        {
            case detectionMode.first:
                return GetFirstEnemy();
            case detectionMode.last:
                return GetLastEnemy();
            case detectionMode.closest:
                return GetClosestEnemy();
            case detectionMode.healthiest:
                return GetHealthiestEnemy();
        }

        return null;
    }

    private Enemy GetFirstEnemy()
    {
        Enemy first = null;

        foreach(Enemy enemy in reachableEnemies)
        {
            if(enemy == null)
            {
                continue;
            }

            if(first == null)
            {
                first = enemy;
            }

            if(enemy.GetProgress() > first.GetProgress())
            {
                first = enemy;
            }
        }

        return first;
    }

    private Enemy GetLastEnemy()
    {
        Enemy last = null;

        foreach (Enemy enemy in reachableEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (last == null)
            {
                last = enemy;
            }

            if (enemy.GetProgress() < last.GetProgress())
            {
                last = enemy;
            }
        }

        return last;
    }

    private Enemy GetClosestEnemy()
    {
        Enemy closest = null;

        foreach (Enemy enemy in reachableEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (closest == null)
            {
                closest = enemy;
            }

            if (Vector2.Distance(transform.position, closest.transform.position) < Vector2.Distance(transform.position, enemy.transform.position))
            {
                closest = enemy;
            }
        }

        return closest;
    }

    private Enemy GetHealthiestEnemy()
    {
        Enemy healthiest = null;

        foreach (Enemy enemy in reachableEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (healthiest == null)
            {
                healthiest = enemy;
            }

            if (enemy.health > healthiest.health)
            {
                healthiest = enemy;
            }
        }

        return healthiest;
    }
}
