using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class DefenseCanvas : SerializedMonoBehaviour
{
    [HideInInspector]
    public UnityEvent NextWavePressed;

    [SerializeField, AssetsOnly]
    private WaveInfo infoTemplate;

    [SerializeField, ChildGameObjectsOnly]
    private Transform infoHolder;

    private Queue<WaveInfo> activeUIWaves = new Queue<WaveInfo>();
    private Animator anim;

    private const int maxQueueCount = 11;
    private const int maxDeQueueCount = 1;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void SetUp(List<EnemyWave> waves)
    {
        for(int i = 0;i < maxQueueCount;i++)
        {
            if(i < waves.Count)
            {
                SpawnUIWave(waves[i], i);
            }
            else
            {
                SpawnUIWave(null, -1);
            }
        }
    }

    public void ShowWaveComplete()
    {
        anim.Play("WaveCompleted");
    }
    public void OnNextWavePressed()
    {
        NextWavePressed?.Invoke();
        anim.Play("WaveStarted");
    }
    public void ProgressUIWaves(List<EnemyWave> waves, int index)
    {
        if(index == 0)
        {
            return;
        }

        if(waves.Count > index + maxQueueCount)
        {
            SpawnUIWave(waves[index + maxQueueCount], index + maxQueueCount);
        }
        else
        {
            SpawnUIWave(null, -1);
        }

        if(index > maxDeQueueCount)
        {
            activeUIWaves.Dequeue().Destroy();
        }
    }

    private WaveInfo SpawnUIWave(EnemyWave wave, float index)
    {
        WaveInfo info = Instantiate(infoTemplate);
        info.Setup(wave, index);
        info.transform.SetParent(infoHolder, false);
        info.gameObject.name = index.ToString();
        activeUIWaves.Enqueue(info);

        return info;
    }
}
