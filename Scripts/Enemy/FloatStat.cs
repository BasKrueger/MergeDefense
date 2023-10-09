using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatStat
{
    [ShowInInspector, HideInEditorMode]
    private float total { get { return totalValue; } }

    [HideInInspector]
    public float totalValue { get { return value * (1 + percentBonus) + flatBonus; } set { } }
    [SerializeField]
    private float value;
    [HideInInspector]
    public float percentBonus;
    [HideInInspector]
    public float flatBonus;

    [ShowInInspector, HideInEditorMode]
    private float percentExtra { get { return percentBonus; } }
    [ShowInInspector, HideInEditorMode]
    private float flatExtra { get { return flatBonus; } }

    public FloatStat(FloatStat other)
    {
        value = other.value;
        percentBonus = other.percentBonus;
        flatBonus = other.flatBonus;
    }

    public static implicit operator float(FloatStat stat)
    {
        return stat.totalValue;
    }

    public static FloatStat operator +(FloatStat a, float b)
    {
        a.flatBonus += b;
        return a;
    }

    public static FloatStat operator -(FloatStat a, float b)
    {
        a.flatBonus -= b;
        return a;
    }

    public static FloatStat operator *(FloatStat a, float b)
    {
        a.percentBonus += b;
        return a;
    }

    public static FloatStat operator /(FloatStat a, float b)
    {
        a.percentBonus -= b;
        return a;
    }
}
