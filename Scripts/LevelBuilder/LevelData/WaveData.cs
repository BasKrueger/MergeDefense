using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    [SerializeField, SuffixLabel("Seconds", true)]
    public float waveDuration = 10f;

    [SerializeField]
    public List<SubWaveData> waveContents = new List<SubWaveData>();

    public WaveData()
    {
        waveContents.Add(new SubWaveData());
    }

    public WaveData(WaveData toClone)
    {
        waveDuration = toClone.waveDuration;
        
        foreach(SubWaveData sub in toClone.waveContents)
        {
            waveContents.Add(new SubWaveData(sub));
        }
    }
}
