using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBoard : MonoBehaviour
{
    [ShowInInspector]
    public LevelData Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            GetComponentInChildren<Path>().SpawnLevelLayout(value);
        }
    }
    [SerializeField, HideInInspector]
    private LevelData level;

    private DefenseCanvas UI;
    private Path path;

    private void Awake()
    {
        UI = GetComponentInChildren<DefenseCanvas>();
        path = GetComponentInChildren<Path>();
    }

    private void Start()
    {
        path.SetUp(level);
        path.WaveCompleted.AddListener(OnWaveCompleted);

        UI.SetUp(GetComponentInChildren<PathEnemies>().EnemyWaves);
        UI.NextWavePressed.AddListener(path.StartNextWave);

        path.StartNextWave();

        void OnWaveCompleted()
        {
            UI.ProgressUIWaves(GetComponentInChildren<PathEnemies>().EnemyWaves, GetComponentInChildren<PathEnemies>().waveIndex);
            UI.ShowWaveComplete();
        }
    }
   
}
