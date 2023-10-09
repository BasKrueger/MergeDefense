using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurretGroup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI turretLevel;
    [SerializeField]
    private TextMeshProUGUI turretCost;

    public void SetTurretLevel(int level)
    {
        turretLevel.text = "Lv." + level.ToString();
    }

    public void SetTurretCost(int cost)
    {
        turretCost.text = cost.ToString();
    }
}
