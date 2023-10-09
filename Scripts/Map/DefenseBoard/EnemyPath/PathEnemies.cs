using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PathEnemies : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent WaveCompleted;

    [SerializeField, AssetsOnly, Required, InlineEditor, FoldoutGroup("References")]
    private Enemy normalEnemy;
    [SerializeField, AssetsOnly, Required, InlineEditor, FoldoutGroup("References")]
    private Enemy fastEnemy;
    [SerializeField, AssetsOnly, Required, InlineEditor, FoldoutGroup("References")]
    private Enemy slowEnemy;

    public List<EnemyWave> EnemyWaves { get; private set; } = new List<EnemyWave>();
    public int waveIndex { get; private set; }

    private int counter = 0;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private WalkableTile startingTile;
    private Dictionary<WaveEnemyType, Enemy> enemies;

    public void SetUp(WalkableTile startingTile, LevelData level)
    {
        this.startingTile = startingTile;

        enemies = new Dictionary<WaveEnemyType, Enemy>
        {
            { WaveEnemyType.Normal, normalEnemy },
            { WaveEnemyType.Fast, fastEnemy },
            { WaveEnemyType.Slow, slowEnemy }
        };
        
        foreach(WaveData wave in level.startingWaves)
        {
            EnemyWaves.Add(new EnemyWave(wave));
        }
        if(level.repeatableWaves.Count == 0)
        {
            Debug.LogError("Error: Can't start level without repeatable waves");
            return;
        }
        EnemyWaves.AddRange(GenerateWaves(100, level.repeatableWaves));
    }

    public void StartNextWave()
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            EnemyWave currentWave = EnemyWaves[waveIndex];
            currentWave.StartWave();

            float time = 0;
            float maxTime = currentWave.GetDuration();

            while (time < maxTime)
            {
                time += Time.deltaTime;

                foreach ((WaveEnemyType type, int level) in currentWave.GetSpawnedEnemiesAtSecond(time))
                {
                    Enemy spawned = SpawnEnemy(enemies[type], level, startingTile);
                    spawned.OnDeath.AddListener(currentWave.OnKill);
                }

                yield return new WaitForEndOfFrame();
            }

            while (activeEnemies.Count > 0)
            {
                yield return new WaitForEndOfFrame();
            }

            currentWave.EndWave();
            WaveCompleted?.Invoke();

            waveIndex++;
        }
    }

    private Enemy SpawnEnemy(Enemy toSpawn, int level, WalkableTile startingTile)
    {
        counter++;

        Enemy activeEnemy = Instantiate(toSpawn);
        activeEnemy.SetUp(startingTile, level);
        activeEnemy.OnDeath.AddListener(OnDeath);

        activeEnemy.transform.SetParent(transform);
        activeEnemy.gameObject.name = counter.ToString();

        activeEnemies.Add(activeEnemy);

        return activeEnemy;
    }
    
    private void OnDeath(Enemy justDied)
    {
        activeEnemies.Remove(justDied);
    }

    private List<EnemyWave> GenerateWaves(int count, List<WaveData> Datas)
    {
        List<EnemyWave> result = new List<EnemyWave>();

        for(int i =0;i < count;i++)
        {
            WaveData toGenerate = Datas[Random.Range(0,Datas.Count - 1)];
            result.Add(new EnemyWave(toGenerate, Mathf.Clamp(i / 3,1,100)));
        }

        return result;
    }
}
