using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWave
{
    private int spawnedEnemies = 0;

    public WaveEnemyType enemyType;
    private int level = 1;

    private Vector2 durationMinMaxPercent = new Vector2(0, 0.25f);
    private float enemiesPerSecond = 1;

    public SubWave(SubWaveData toCopy)
    {
        this.enemyType = toCopy.enemyType;
        this.level = toCopy.level;
        this.durationMinMaxPercent = toCopy.duration;
        this.enemiesPerSecond = toCopy.enemiesPerSecond;
    }

    public (bool, int) CanSpawn(float time, float waveDuration)
    {
        Vector2 durationMinMax = GetDuration(waveDuration);

        if (time < durationMinMax.x || time > durationMinMax.y)
        {
            return (false, level);
        }

        if (Mathf.FloorToInt((time - durationMinMax.x) / (1 / enemiesPerSecond)) >= spawnedEnemies)
        {
            spawnedEnemies++;
            return (true, level);
        }

        return (false, level);
    }

    public Vector2 GetDuration(float waveDuration)
    {
        return durationMinMaxPercent * waveDuration;
    }

    public float GetSpawnsLeft(float waveDuration)
    {
        float total = GetTotalSpawns(waveDuration);
        float left = total - spawnedEnemies;

        return 1 - (left / total);
    }

    public float GetTotalSpawns(float time)
    {
        if (durationMinMaxPercent.x == 0 && durationMinMaxPercent.y == 1)
        {
            return enemiesPerSecond * time;
        }

        float delta = GetDuration(time).y - GetDuration(time).x;
        return Mathf.FloorToInt(delta / (1 / enemiesPerSecond)) + 1;
    }
}