using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUI : MonoBehaviour
{
    [SerializeField]
    private Transform turretLevelPosition;
    private TurretUILevel level;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        level = GetComponentInChildren<TurretUILevel>();
    }

    public void SetUp(float turretLevel)
    {
        level.UpdateValue(turretLevel);
    }

    public void UpdateLevel(float value)
    {
        level.UpdateValue(value);
    }

    private void LateUpdate()
    {
        level.UpdatePosition(turretLevelPosition, rect);
    }
}
