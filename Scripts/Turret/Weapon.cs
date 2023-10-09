using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : SerializedMonoBehaviour
{
    [SerializeField]
    private IntStat damage;
    [SerializeField]
    private FloatStat fireRate;
    [SerializeField]
    private FloatStat projectileSpeed;

    [SerializeField, Required]
    private Projectile projectile;
    [OdinSerialize]
    private Dictionary<int, Gun> gunUnlocks = new Dictionary<int, Gun>();

    private float activeFireCooldown = -1;
    private Dictionary<detectionMode, Enemy> customTargets;
    private Enemy defaultTarget;

    private const int damageUpgrade = 1;
    private const float fireRateUpgrade = 0.1f;

    public void SetUp(int level)
    {
        damage.percentBonus = level - 1;
        foreach (KeyValuePair<int, Gun> pair in gunUnlocks)
        {
            pair.Value.SetUp(fireRate);
        }
        gunUnlocks[1].Unlock();

        UnlockGunsUpToLevel(level);
    }

    public void WeaponUpdate(Dictionary<detectionMode, Enemy> customTargets, Enemy defaultTarget)
    {
        this.customTargets = customTargets;
        this.defaultTarget = defaultTarget;

        activeFireCooldown -= Time.deltaTime;

        if(activeFireCooldown <= 0 && defaultTarget != null)
        {
            StartCoroutine(Fire());
            activeFireCooldown = 1 / fireRate;
        }
    }

    private IEnumerator Fire()
    {
        foreach (Gun gun in GetUnlockedGuns())
        {
            int customTarget = gun.GetCustomDetection();
            if(customTarget == -1)
            {
                if(defaultTarget == null)
                {
                    continue;
                }
                gun.TryFire(projectile, projectileSpeed, damage, defaultTarget);
            }
            else
            {
                if(customTargets == null)
                {
                    continue;
                }
                gun.TryFire(projectile, projectileSpeed, damage, customTargets[(detectionMode)customTarget]);
            }
            yield return new WaitForSeconds((1 / fireRate * 0.3f) / GetUnlockedGuns().Count); 
        }
    }

    public void LevelUp()
    {
        damage += damageUpgrade;
        fireRate += fireRateUpgrade;

        foreach(Gun gun in GetUnlockedGuns())
        {
            gun.SetFireRate(fireRate);
        }
    }

    public bool UnlockGunsUpToLevel(float level)
    {
        bool newUnlock = false;
        foreach (KeyValuePair<int, Gun> pair in gunUnlocks)
        {
            if (pair.Key <= level)
            {
                pair.Value.Unlock(1.5f);

                if (pair.Key == level)
                {
                    newUnlock = true;
                }
            }
        }

        return newUnlock;
    }

    public void Dissolve(float duration)
    {
        foreach(Gun gun in GetUnlockedGuns())
        {
            gun.Dissolve(duration);
        }
    }

    public void Manifest(float manifestationTime = 1f, float finalizeManifestationTime = 0.3f)
    {
        int index = 0;
        foreach(Gun gun in GetComponentsInChildren<Gun>())
        {
            if (index == 0) 
            { 
                gun.Manifest(0, manifestationTime, finalizeManifestationTime);
            }
            else
            {
                gun.Lock();
            }
            index++;
        }
    }

    private List<Gun> GetUnlockedGuns(int level = -1)
    {
        List<Gun> result = new List<Gun>();

        if(level > 0)
        {
            result.Add(gunUnlocks[level]);
        }
        else
        {
            foreach (KeyValuePair<int, Gun> pair in gunUnlocks)
            {
                if (pair.Value.unlocked)
                {
                    result.Add(pair.Value);
                }
            }
        }
       

        return result;
    }

    public bool RotateGunsToTarget(Dictionary<detectionMode, Enemy> customTargets, Enemy defaultTarget, float rotationSpeed)
    {
        bool result = false;
        foreach(Gun gun in GetUnlockedGuns())
        {
            Enemy realTarget = defaultTarget;

            int customDetection = gun.GetCustomDetection();
            if(customDetection == -1)
            {
                realTarget = defaultTarget;
            }
            else
            {
                realTarget = customTargets[(detectionMode)customDetection];
            }

            if(RotateGun(realTarget, gun))
            {
                result = true;
            }

        }
        return result;

        bool RotateGun(Enemy enemy, Gun gun)
        {
            if (enemy == null || Vector3.Angle(enemy.transform.position - gun.transform.position, transform.forward) <= 1)
            {
                return false;
            }

            Vector3 direction = enemy.transform.position - gun.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            gun.transform.rotation = Quaternion.RotateTowards(gun.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            return true;
        }
    }
}
