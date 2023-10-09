using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTile : TurretTile, IDropable
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void OnDropped(GameObject origin)
    {
        TurretTile originalTile = origin.GetComponent<TurretTile>();
        if (originalTile == null || originalTile.turret == null)
        {
            return;
        }

        GetTurretFrom(originalTile);
        DestroyTurret();
    }

    public override void StartDragHighlight(Turret originalTurret)
    {
        anim?.SetBool("open", true);
        base.StartDragHighlight(originalTurret);
    }

    public override void StopDragHighlight()
    {
        anim?.SetBool("open", false);
        base.StopDragHighlight();
    }
}
