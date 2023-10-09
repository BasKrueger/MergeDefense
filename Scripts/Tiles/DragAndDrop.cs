using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IDropable
{
    void OnDropped(GameObject origin);
}

public class DragAndDrop 
{
    public UnityEvent<Vector3> DragStarted;
    public UnityEvent<Vector3> Dragged;
    public UnityEvent<Vector3> DragEnded;
    public UnityEvent<GameObject> dropped;

    public DragPhase phase = DragPhase.end;

    private GameObject gameObject;

    private int defaultLayer;
    private int selectedLayer;
    private int excludedLayers;

    public void SetUp(GameObject gameObject, int selectedLayer, List<int> excludedLayers)
    {
        DragStarted = new UnityEvent<Vector3>();
        DragEnded = new UnityEvent<Vector3>();
        Dragged = new UnityEvent<Vector3>();
        dropped = new UnityEvent<GameObject>();


        this.gameObject = gameObject;
        this.selectedLayer = selectedLayer;
        defaultLayer = gameObject.layer;

        this.excludedLayers = 0;
        foreach(int layer in excludedLayers)
        {
            this.excludedLayers += layer;
        }
    }

    public void DragAndDropUpdate()
    {
        TryDragStart();
        TryOnDrag();
        TryOnDragEnd();
    }


    private void OnDragStart(Vector3 dragPosition)
    {
        phase = DragPhase.start;

        gameObject.layer = selectedLayer;
        DragStarted?.Invoke(dragPosition);
    }

    private void OnDrag(Vector3 DragPosition)
    {
        phase = DragPhase.dragging;
        Dragged?.Invoke(DragPosition);
    }

    private void OnDragEnd(Vector3 DragPosition)
    {
        phase = DragPhase.end;

        GetDropTarget()?.OnDropped(gameObject);

        DragEnded?.Invoke(DragPosition);
        gameObject.layer = defaultLayer;
    }

    #region TryToUpdatePhases
    private bool TryDragStart()
    {
        if (phase != DragPhase.end || !OverThisGameObject())
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnDragStart(GetDragPosition());
            return true;
        }
        return false;

        bool OverThisGameObject()
        {
            if (GetDragPosition() == new Vector3())
            {
                return false;
            }

            RaycastHit hit = new RaycastHit();
            LayerMask mask = 1 << defaultLayer;
            Vector3 direction = GetDragPosition() - Camera.main.transform.position;

            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity, mask))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private bool TryOnDrag()
    {
        if (phase == DragPhase.dragging)
        {
            OnDrag(GetDragPosition());
            return true;
        }

        if (phase == DragPhase.end)
        {
            return false;
        }

        if (Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            OnDrag(GetDragPosition());
            return true;
        }
        return false;
    }

    private bool TryOnDragEnd()
    {
        if (phase != DragPhase.dragging)
        {
            return false;
        }

        if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            OnDragEnd(GetDragPosition());
            return true;
        }
        return false;
    }

    private Vector3 GetDragPosition()
    {
        Vector3 target = new Vector3();

        if (GetMousePosition() != new Vector3())
        {
            target = GetMousePosition();
        }
        else if (GetTouchPosition() != new Vector3())
        {
            target = GetTouchPosition();
        }
        else
        {
            return new Vector3();
        }

        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(target);
        LayerMask mask = 1 << excludedLayers;
        mask = ~mask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            return hit.point;
        }

        return new Vector3();


        Vector3 GetMousePosition()
        {
            return Input.mousePosition;
        }

        Vector3 GetTouchPosition()
        {
            if (Input.touchCount == 0)
            {
                return new Vector3();
            }
            return Input.GetTouch(0).position;
        }
    }


    #endregion

    private IDropable GetDropTarget()
    {
        if (GetDragPosition() == new Vector3())
        {
            return null;
        }

        RaycastHit hit = new RaycastHit();
        LayerMask mask = 1 << defaultLayer;
        Vector3 direction = GetDragPosition() - Camera.main.transform.position;

        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity, mask))
        {
            if(hit.transform.GetComponent<IDropable>() != null)
            {
                return hit.transform.GetComponent<IDropable>();
            }
        }

        return null;
    }
}
