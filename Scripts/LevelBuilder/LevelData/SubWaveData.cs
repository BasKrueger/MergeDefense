using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubWaveData
{
    [EnumToggleButtons, HideLabel, HorizontalGroup("type", Width = 0.75f)]
    public WaveEnemyType enemyType;
    [SerializeField,HorizontalGroup("type"), HideLabel, SuffixLabel("Enemy level", true)]
    public int level = 1;

    [SerializeField, HorizontalGroup("Enemies", Width = 0.55f), HideLabel, MinMaxSlider(0, 1)]
    public Vector2 duration = new Vector2(0, 0.25f);
    [SerializeField, HorizontalGroup("Enemies"), HideLabel, SuffixLabel("Enemies per Second", true)]
    public float enemiesPerSecond = 1;

    public SubWaveData()
    {

    }

    public SubWaveData(SubWaveData toClone)
    {
        enemyType = toClone.enemyType;
        level = toClone.level;
        duration = toClone.duration;
        enemiesPerSecond = toClone.enemiesPerSecond;
    }
}
