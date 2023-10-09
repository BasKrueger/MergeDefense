using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class Gun : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly]
    private Transform gunPoint;

    [SerializeField]
    private bool customDetectionMode = false;
    [SerializeField, EnumToggleButtons, ShowIf("customDetectionMode"), HideLabel]
    private detectionMode detection;

    private Animator anim;
    private AudioSource audio_;

    private Projectile projectile;
    private float projectileSpeed;
    private int damage;
    private Enemy lastEnemy;
    private Vector3 lastEnemyPosition;

    public bool unlocked { get; private set; } = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audio_ = GetComponent<AudioSource>();
    }

    public virtual void SetUp(float fireRate)
    {
        anim.SetFloat("Speed",fireRate);
        Lock();
    }

    public void Lock()
    {
        unlocked = false;
        foreach (Material mat in GetAllMaterials())
        {
            mat.SetFloat("_DissolveWidth", 1);
            mat.SetFloat("_DissolveAmount", 1);
        }
    }

    public void SetFireRate(float fireRate)
    {
        anim.SetFloat("Speed", fireRate);
    }

    public void Unlock(float delay = 0)
    {
        if (unlocked)
        {
            return;
        }

        Manifest(delay);
        unlocked = true;
    }

    public virtual void TryFire(Projectile projectile, float projectileSpeed, int damage, Enemy enemy)
    {
        if(enemy == null)
        {
            return;
        }

        this.projectile = projectile;
        this.projectileSpeed = projectileSpeed;
        this.damage = damage;
        this.lastEnemyPosition = enemy.transform.position;
        this.lastEnemy = enemy;

        anim.SetBool("Shoot", true);
    }

    //called via animation event
    protected virtual void Fire()
    {
        anim.SetBool("Shoot", false);

        Projectile activeProjectile = Instantiate(projectile);
        activeProjectile.transform.position = gunPoint.position;
        activeProjectile.transform.rotation = transform.rotation;
        activeProjectile.Setup(damage, projectileSpeed, lastEnemyPosition, lastEnemy);
        GetComponentInChildren<ParticleSystem>().Play();

        PlayAudio();
    }

    private void PlayAudio()
    {
        GameObject sound = new GameObject("shootsound");
        Destroy(sound, 3);
        AudioSource activeAudio = sound.AddComponent<AudioSource>();

        activeAudio.clip = audio_.clip;
        activeAudio.loop = audio_.loop;
        activeAudio.volume = audio_.volume;
        activeAudio.pitch = audio_.pitch;

        activeAudio.transform.SetParent(transform);

        activeAudio.Play();
    }


    public void Manifest(float delay = 0, float manifestationTime = 1f, float finalizeManifestationTime = 0.3f)
    {
        foreach (Material mat in GetAllMaterials())
        {
            StartCoroutine(manifest(mat));
        }

        IEnumerator manifest(Material mat)
        {
            yield return new WaitForSeconds(delay);

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

    public void Dissolve(float dissolveTime)
    {
        dissolveTime = 0.25f;

        foreach (Material mat in GetAllMaterials())
        {
            StartCoroutine(dissolve(mat));
        }

        IEnumerator dissolve(Material mat)
        {
            mat.SetFloat("_DissolveWidth", 0.2f);
            float time = 0;

            while (time < dissolveTime)
            {
                mat.SetFloat("_DissolveAmount", time / dissolveTime);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public int GetCustomDetection()
    {
        if (!customDetectionMode)
        {
            return -1;
        }

        return (int)detection;
    }

    private List<Material> GetAllMaterials()
    {
        List<Material> mats = new List<Material>();
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            mats.AddRange(renderer.materials);
        }
        return mats;
    }
}
