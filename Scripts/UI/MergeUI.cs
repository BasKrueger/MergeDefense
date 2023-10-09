using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MergeUI : SerializedMonoBehaviour
{
    public Transform coinCounterTarget;
    public List<Transform> coinUpgradeTarget;

    private CoinCounter coinCounter;
    private TurretGroup turretGroup;

    [SerializeField]
    private TextMeshProUGUI levelUpCost;
    [SerializeField]
    private TextMeshProUGUI levelUpLevel;

    private void Awake()
    {
        turretGroup = GetComponentInChildren<TurretGroup>();
        coinCounter = GetComponentInChildren<CoinCounter>();
    }

    public void SetTurretLevel(int level)
    {
        turretGroup.SetTurretLevel(level);
        levelUpLevel.text = "Lv." + (level + 1).ToString();
    }

    public void SetTurretCost(int cost)
    {
        turretGroup.SetTurretCost(cost);
    }

    public void SetLevelUpCost(int cost)
    {
        levelUpCost.text = cost.ToString();
    }

    public void SetCoinCounterGlow(bool state)
    {
        if (state)
        {
            coinCounter.glow.StartGlow();
        }
        else
        {
            coinCounter.glow.EndGlow();
        }
    }

    public void BlinkCoinCounter()
    {
        coinCounter.glow.Blink();
    }

    public void UpdateCoinCounterValue(float value)
    {
        coinCounter.UpdateCount(value);
    }
}
