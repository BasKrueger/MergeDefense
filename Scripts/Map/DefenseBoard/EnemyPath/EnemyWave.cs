using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWave
{
    [HideInInspector]
    public UnityEvent WaveStarted = new UnityEvent();
    [HideInInspector]
    public UnityEvent<float> SpawnUpdated = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<float> KillUpdated = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent WaveFinished = new UnityEvent();

    private float waveDuration = 10f;
    private int waveBonusLevels = 0;
    private List<SubWave> waveContents = new List<SubWave>();

    private int killedEnemies = 0;

    public EnemyWave(WaveData other, int bonusLevels = 0)
    {
        this.waveBonusLevels = bonusLevels;
        this.waveDuration = other.waveDuration;
        waveContents = new List<SubWave>();
        foreach(SubWaveData subwave in other.waveContents)
        {
            waveContents.Add(new SubWave(subwave));
        }
    }

    public void StartWave()
    {
        killedEnemies = 0;
        WaveStarted?.Invoke();
    }

    public void OnKill(Enemy killed)
    {
        killedEnemies++;

        float total = 0;
        foreach(SubWave wave in waveContents)
        {
            total += wave.GetTotalSpawns(waveDuration);
        }

        KillUpdated?.Invoke(killedEnemies / total);
    }

    public void EndWave()
    {
        WaveFinished?.Invoke();
    }

    public List<(WaveEnemyType, int)> GetSpawnedEnemiesAtSecond(float time)
    {
        if(time > waveDuration)
        {
            return new List<(WaveEnemyType, int)>();
        }

        List<(WaveEnemyType, int)> result = new List<(WaveEnemyType, int)>();

        foreach(SubWave subWave in waveContents)
        {
            (bool spawning, int level) pair = subWave.CanSpawn(time, waveDuration);
            if (pair.spawning)
            {
                result.Add((subWave.enemyType, pair.level + waveBonusLevels));
            }
        }

        #region Update event
        float totalPercent = 0;
        foreach(SubWave subWave in waveContents)
        {
            totalPercent += subWave.GetSpawnsLeft(waveDuration);
        }
        totalPercent /= (float)waveContents.Count;
        SpawnUpdated?.Invoke(totalPercent);
        #endregion

        return result;
    }

    public float GetDuration()
    {
        float highest = 0;
        foreach (SubWave subWave in waveContents)
        {
            if(subWave.GetDuration(waveDuration).y > highest)
            {
                highest = subWave.GetDuration(waveDuration).y;
            }
        }

        return highest;
    }
}
