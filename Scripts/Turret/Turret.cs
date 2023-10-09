using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References"),ChildGameObjectsOnly]
    private List<MeshRenderer> spawnManifestRenders;

    [Title("Settings")]
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float range;
    [SerializeField]
    private float manifestSpeedModifier = 1;

    [Title("")]
    [SerializeField, EnumToggleButtons]
    public TurretType type;
    [SerializeField, EnumToggleButtons]
    private RotationMode rotationMode;

    private DetectionRange detection;
    private TurretUI ui;
    private TurretParticles particles;
    private Weapon weapon;
    private TurretSoundEffects sounds;

    public int level { get;private set; } = 1;

    private void Awake()
    {
        detection = GetComponentInChildren<DetectionRange>();
        ui = GetComponentInChildren<TurretUI>();
        particles = GetComponentInChildren<TurretParticles>();
        weapon = GetComponentInChildren<Weapon>();
        sounds = GetComponentInChildren<TurretSoundEffects>();
    }

    public void SetUp(int startLevel)
    {
        level = startLevel;

        weapon.SetUp(level);
        detection.SetUp(range);
        ui.SetUp(level);
        sounds.PlaySound(turretSounds.spawn);
        StartCoroutine(Manifest());

        GetComponentInChildren<Camera>().enabled = false;
    }

    public void TurretUpdate()
    {
        Enemy defaultTarget = detection.GetEnemy();
        Dictionary<detectionMode, Enemy> customTargets = new Dictionary<detectionMode, Enemy>();

        for(int i = 0;i < System.Enum.GetValues(typeof(detectionMode)).Length; i++)
        {
            customTargets.Add((detectionMode)i, detection.GetEnemy(i));
        }

        RotateToTarget(defaultTarget);
        weapon.WeaponUpdate(customTargets, defaultTarget);
    }

    public void Destroy()
    {
        Destroy(this.gameObject,3);
        Destroy(ui.gameObject);
        StartCoroutine(Dissolve(2));
    }

    public bool Merge(Turret other)
    {
        if(!CanMerge(other))
        {
            return false;
        }
        other.Destroy();
        LevelUp();
        return true;
    }

    public bool CanMerge(Turret other)
    {
        if (other == this)
        {
            return false;
        }
        if (other.level != level)
        {
            return false;
        }
        if(other.type != this.type)
        {
            return false;
        }

        return true;
    }

    private void LevelUp()
    {
        level++;
        ui.UpdateLevel(level);
        weapon.LevelUp();

        if (weapon.UnlockGunsUpToLevel(level))
        {
            sounds.PlaySound(turretSounds.bigUpgrade);
            particles.Play(TurretParticle.bigLevelUp);
        }
        else
        {
            sounds.PlaySound(turretSounds.upgrade);
            particles.Play(TurretParticle.levelUp);
        }
    }

    private bool RotateToTarget(Enemy enemy)
    {
        switch (rotationMode)
        {
            case RotationMode.Self:
                return RotateTurretToTarget();
            case RotationMode.Weapon:
                return RotateWeaponToTarget();
            case RotationMode.Guns:
                return RotateGunsToTarget();
        }

        return true;
        bool RotateTurretToTarget()
        {
            if (enemy == null || Vector3.Angle(enemy.transform.position - transform.position, transform.forward) <= 1)
            {
                return false;
            }

            Vector3 direction = enemy.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            return true;
        }
        bool RotateWeaponToTarget()
        {
            if (enemy == null || Vector3.Angle(enemy.transform.position - weapon.transform.position, transform.forward) <= 1)
            {
                return false;
            }

            Vector3 direction = enemy.transform.position - weapon.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            weapon.transform.rotation = Quaternion.RotateTowards(weapon.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            return true;
        }
        bool RotateGunsToTarget()
        {
            Dictionary<detectionMode, Enemy> customTargets = new Dictionary<detectionMode, Enemy>();
            Enemy defaultTarget = detection.GetEnemy();
            for (int i = 0; i < System.Enum.GetValues(typeof(detectionMode)).Length; i++)
            {
                customTargets.Add((detectionMode)i, detection.GetEnemy(i));
            }

            return weapon.RotateGunsToTarget(customTargets, defaultTarget, rotationSpeed);
        }
    }

    public IEnumerator Manifest(float manifestationTime = 1f, float finalizeManifestationTime = 0.3f, bool weaponToo = false)
    {
        manifestationTime *= 1 / manifestSpeedModifier;
        finalizeManifestationTime *= 1 / manifestSpeedModifier;

        if (weaponToo)
        {
            weapon.Manifest(manifestationTime, finalizeManifestationTime);
        }

        foreach (Material mat in GetMaterials())
        {
            StartCoroutine(manifest(mat));
        }

        yield return new WaitForSeconds(Mathf.RoundToInt(manifestationTime + finalizeManifestationTime));

        IEnumerator manifest(Material mat)
        {
            mat.SetFloat("_DissolveWidth", 0.3f);
            float time = manifestationTime;

            while (time > 0)
            {
                mat.SetFloat("_DissolveAmount", time / manifestationTime);
                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            time = finalizeManifestationTime;
            float startingWidth = mat.GetFloat("_DissolveWidth");

            while (time >= 0)
            {
                mat.SetFloat("_DissolveWidth", (time / finalizeManifestationTime) * startingWidth);
                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            mat.SetFloat("_DissolveWidth", 0);
        }
    }

    public IEnumerator Dissolve(float dissolveTime)
    {
        dissolveTime *= 1 / manifestSpeedModifier;

        weapon.Dissolve(dissolveTime);

        foreach(Material mat in GetMaterials())
        {
            mat.SetFloat("_DissolveWidth", 0.2f);
        }
        float time = 0;

        while (time < dissolveTime)
        {
            foreach(Material mat in GetMaterials())
            {
                mat.SetFloat("_DissolveAmount", time / dissolveTime);
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private List<Material> GetMaterials()
    {
        List<Material> mats = new List<Material>();
        foreach (MeshRenderer render in spawnManifestRenders)
        {
            foreach (Material mat in render.materials)
            {
                mats.Add(mat);
            }
        }
        return mats;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range / 2);
    }
}
