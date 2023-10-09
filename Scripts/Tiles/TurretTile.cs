using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretTile : BoardTile
{
    [HideInInspector]
    public Turret turret;
    [SerializeField]
    protected Transform turretPosition;
    [HideInInspector]
    public bool lockDragDrop = false;

    protected void GetTurretFrom(TurretTile origin)
    {
        if (turret != null)
        {
            return;
        }

        if (CanRegisterTurret())
        {
            RegisterTurret(origin.turret);
            origin.UnRegisterTurret();
        }
    }

    public virtual bool RegisterTurret(Turret turret)
    {
        if (!CanRegisterTurret())
        {
            return false;
        }
        this.turret = turret;
        turret.transform.SetParent(turretPosition);
        turret.transform.position = turretPosition.position;

        return true;
    }

    public bool CanRegisterTurret()
    {
        return this.turret == null;
    }

    protected void UnRegisterTurret()
    {
        turret = null;
    }

    protected virtual void DestroyTurret()
    {
        if(turret == null)
        {
            return;
        }

        Turret turr = turret;
        UnRegisterTurret();
        turr.Destroy();
    }

    public virtual void StartDragHighlight(Turret originalTurret)
    {
        
    }

    public virtual void StopDragHighlight()
    {

    }
}
