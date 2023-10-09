using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[System.Serializable]
public class IntStat
{
    [ShowInInspector, HideInEditorMode]
    private float total { get { return totalValue; } }

    private int totalValue { get { return Mathf.RoundToInt(value * (1 + percentBonus)) + flatBonus; } set { } }
    [SerializeField]
    public int value;
    [HideInInspector]
    public float percentBonus;
    [HideInInspector]
    public int flatBonus;

    [ShowInInspector, HideInEditorMode]
    private float percentExtra { get { return percentBonus; } }
    [ShowInInspector, HideInEditorMode]
    private int flatExtra { get { return flatBonus; } }

    public IntStat(IntStat other)
    {
        value = other.value;
        percentBonus = other.percentBonus;
        flatBonus = other.flatBonus;
    }

    public static implicit operator int(IntStat stat)
    {
        return stat.totalValue;
    }

    public static IntStat operator +(IntStat a, int b)
    {
        a.flatBonus += b;
        return a;
    }

    public static IntStat operator -(IntStat a, int b)
    {
        a.flatBonus -= b;
        return a;
    }

    public static IntStat operator *(IntStat a, int b)
    {
        a.percentBonus += b;
        return a;
    }

    public static IntStat operator /(IntStat a, int b)
    {
        a.percentBonus -= b;
        return a;
    }
}
