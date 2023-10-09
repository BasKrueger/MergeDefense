using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkableTile : BoardTile
{
    [HideInInspector,SerializeField]
    private WalkableTile next;

    public void SetUpWalkable(WalkableTile next)
    {
        this.next = next;
    }

    public WalkableTile GetNextTile()
    {
        return next;
    }
}
