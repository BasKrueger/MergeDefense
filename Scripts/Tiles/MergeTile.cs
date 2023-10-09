using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.UI.Image;

public class MergeTile : TurretTile, IDropable
{
    [SerializeField]
    private List<UnityEngine.Color> mergeableColors;
    [SerializeField]
    private List<UnityEngine.Color> notMergableColors;

    private DragAndDrop dragAndDrop;
    private Vector3 turretStartPosition;

    public override void SetUp(Vector2 id = new Vector2())
    {
        base.SetUp(id);

        dragAndDrop = new DragAndDrop();
        dragAndDrop.SetUp(this.gameObject, LayerMask.NameToLayer("SelectedTile"), new List<int>() { LayerMask.NameToLayer("Turret") });

        #region Connect Drag and Drop Events
        dragAndDrop.DragStarted.AddListener(SetTurretStartPosition);
        dragAndDrop.DragStarted.AddListener(ActivateHighlights);
        dragAndDrop.Dragged.AddListener(UpdateTurretPosition);
        dragAndDrop.DragEnded.AddListener(ResetTurretPosition);
        dragAndDrop.DragEnded.AddListener(DeActivateHighlights);

        void SetTurretStartPosition(Vector3 dragPosition)
        {
            if (turret != null)
            {
                turretStartPosition = turret.transform.position;
            }
        }

        void UpdateTurretPosition(Vector3 dragPosition)
        {
            if (turret != null)
            {
                turret.transform.position = dragPosition + new Vector3(0,0.25f,0);
            }
        }

        void ResetTurretPosition(Vector3 dragPosition)
        {
            if (turret != null)
            {
                turret.transform.position = turretStartPosition;
            }
        }

        void ActivateHighlights(Vector3 dragPosition)
        {
            if(turret != null)
            {
                foreach (TurretTile tile in FindObjectsOfType<TurretTile>())
                {
                    tile.StartDragHighlight(this.turret);
                }
            }
        }

        void DeActivateHighlights(Vector3 dragPosition)
        {
            foreach (TurretTile tile in FindObjectsOfType<TurretTile>())
            {
                tile.StopDragHighlight();
            }
        }
        #endregion
    }

    public override void StartDragHighlight(Turret turret)
    {
        if(turret == null || this.turret == null || this.turret == turret)
        {
            return;
        }

        if(this.turret.CanMerge(turret))
        {
            base.StartBlinking(mergeableColors);
        }
        else
        {
            base.StartBlinking(notMergableColors);
        }
    }

    public override void StopDragHighlight()
    {
        base.StopBlinking();
        base.StopDragHighlight();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!lockDragDrop)
        {
            dragAndDrop?.DragAndDropUpdate();
        }
    }

    public void OnDropped(GameObject origin)
    {
        if (lockDragDrop)
        {
            return;
        }

        TurretTile originalTile = origin.GetComponent<TurretTile>();
        if(originalTile == null || originalTile.turret == null)
        {
            return;
        }

        if (base.CanRegisterTurret())
        {
            base.GetTurretFrom(originalTile);
        }
        else
        {
            if (base.turret.Merge(originalTile.turret))
            {
                originalTile.turret = null;
            }
        }
    }
}
