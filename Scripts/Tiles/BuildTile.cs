using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTile : TurretTile, IDropable
{
    [SerializeField]
    private List<Color> buildableColor;
    [SerializeField]
    private List<Color> mergeableColor;
    [SerializeField]
    private List<Color> upgradeableColor;
    [SerializeField]
    private List<Color> nonInteractableColor;
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(base.turret != null)
        {
            base.turret.TurretUpdate();
        }
    }

    public override void StartDragHighlight(Turret turret)
    {
        if (CanAddTurret())
        {
            base.StartBlinking(buildableColor);
        }
        else if (CanMergeTurret(turret))
        {
            base.StartBlinking(mergeableColor);
        }
        else if(CanUpgradeTurret(turret))
        {
            base.StartBlinking(upgradeableColor);
        }
        else
        {
            base.StartBlinking(nonInteractableColor);
        }
    }

    public override void StopDragHighlight()
    {
        base.StopBlinking();
        base.StopDragHighlight();
    }


    public void OnDropped(GameObject origin)
    {
        if (base.lockDragDrop)
        {
            return;
        }

        TurretTile originalTile = origin.GetComponent<TurretTile>();
        if (originalTile == null || originalTile.turret == null)
        {
            return;
        }

        if (CanAddTurret())
        {
            base.GetTurretFrom(originalTile);
            return;
        }

        if (CanMergeTurret(originalTile.turret))
        {
            base.turret.Merge(originalTile.turret);
            originalTile.turret = null;
        }
        else if(CanUpgradeTurret(originalTile.turret))
        {
            base.DestroyTurret();
            base.GetTurretFrom(originalTile);
        }
    }

    private bool CanAddTurret()
    {
        return base.turret == null;
    }

    private bool CanMergeTurret(Turret other)
    {
        if(base.turret == null || other == null)
        {
            return false;
        }

        return base.turret.CanMerge(other);
    }

    private bool CanUpgradeTurret(Turret other)
    {
        if (base.turret == null || other == null)
        {
            return false;
        }

        return other.level > base.turret.level || other.type != base.turret.type;
    }
}
