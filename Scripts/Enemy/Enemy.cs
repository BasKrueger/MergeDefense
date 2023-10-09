using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : SerializedMonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Enemy> OnDeath;
    [HideInInspector]
    public UnityEvent<Enemy, int, Projectile> OnTakeDamage;

    [SerializeField,FoldoutGroup("References"), HideInInlineEditors]
    private Coin coinTemplate;
    [SerializeField, FoldoutGroup("References"), HideInInlineEditors]
    private Canvas damageCanvas;

    [ShowInInspector, HideInEditorMode]
    public IntStat maxHealth;
    [SerializeField]
    public IntStat health;
    [SerializeField]
    public FloatStat speed;
    [SerializeField]
    public IntStat coinValue;

    private WalkableTile lastTile;
    private WalkableTile nextTile;
    private EnemyMesh mesh;
    private EnemySounds sound;

    private int TileProgress = 0;
    private int level = 0;

    private void Awake()
    {
        mesh = GetComponentInChildren<EnemyMesh>();
        sound = GetComponentInChildren<EnemySounds>();
    }

    public void SetUp(WalkableTile startingPoint, int level)
    {
        level = Mathf.Clamp(level - 1, 0, 100);

        this.level = level;
        health.percentBonus = level * 3;
        maxHealth = new IntStat(health);

        transform.position = startingPoint.transform.position;
        lastTile = startingPoint;
        nextTile = startingPoint.GetNextTile();

        mesh.TurnAt(nextTile.transform.position);
    }

    public void TakeDamage(int damage, Projectile source)
    {
        if(health <= 0 || damage <= 0)
        {
            return;
        }

        OnTakeDamage?.Invoke(this, damage, source);

        health -= damage;

        sound.PlaySound(enemySound.TakeDamage);
        mesh.SetRedPercent(health / (float)maxHealth);

        Canvas damageNumber = Instantiate(damageCanvas);
        damageNumber.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        damageNumber.transform.position = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), 0, 0) + new Vector3(0,source.transform.position.y,0);
        Destroy(damageNumber.gameObject, 1);

        if (health <= 0)
        {
            mesh.TurnAt((source.transform.position - transform.position).normalized + transform.position);
            Die();
        }
    }

    public void TakeDamage(int damage, Debuff source)
    {
        if (health <= 0 || damage <= 0)
        {
            return;
        }

        health -= damage;

        sound.PlaySound(enemySound.TakeDamage);
        mesh.SetRedPercent(health / (float)maxHealth);

        Canvas damageNumber = Instantiate(damageCanvas);
        damageNumber.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        damageNumber.transform.position = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), 0, 0) + new Vector3(0, source.transform.position.y, 0);
        Destroy(damageNumber.gameObject, 1);

        if (health <= 0)
        {
            mesh.TurnAt((source.transform.position - transform.position).normalized + transform.position);
            Die();
        }
    }

    private void Update()
    {
        if(health <= 0)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        if(nextTile == null)
        {
            OnReachDestination();
            return;
        }

        if (Vector3.Distance(transform.position,nextTile.transform.position) > 0.05f)
        {
            transform.position += (nextTile.transform.position - transform.position).normalized * Mathf.Clamp(speed, 0.1f,Mathf.Infinity) * Time.deltaTime;
            TileProgress++;
        }
        else
        {
            lastTile = nextTile;
            nextTile = nextTile.GetNextTile();
            if(nextTile != null)
            {
                mesh.TurnAt(nextTile.transform.position);
            }
        }
    }

    private void OnReachDestination()
    {
        Die();
    }

    private void Die()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        OnDeath?.Invoke(this);
        DropCoins();

        sound.PlaySound(enemySound.Die);
        mesh.PlayAnimation(EnemyAnimation.Die);
        GetComponentInChildren<ParticleSystem>().Play();

        Destroy(GetComponent<Collider>());
        Destroy(this.gameObject,5);
        Destroy(this);
    }

    public float GetProgress()
    {
        if(lastTile == null || nextTile == null)
        {
            return 0;
        }
        return TileProgress + Vector3.Distance(lastTile.transform.position, nextTile.transform.position) - Vector3.Distance(transform.position, nextTile.transform.position);
    }

    private void DropCoins()
    {
        for(int i = 0;i < level/4 + 1; i++)
        {
            Coin activeCoin = Instantiate(coinTemplate);
            activeCoin.Setup(transform.position + new Vector3(0, 0.25f, 0));
            activeCoin.value = coinValue;

            activeCoin.Collided.AddListener(FindObjectOfType<MergeBoard>().AddCoin);
        }
    }
}
