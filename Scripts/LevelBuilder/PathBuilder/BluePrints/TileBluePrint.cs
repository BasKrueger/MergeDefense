using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileBluePrint
{
    public TileType type;
    public Vector2 id;

    public virtual Color GetPlacedColor()
    {
        return Color.gray;
    }
    public virtual Color GetPlaceableColor()
    {
        return Color.gray;
    }
    public virtual Color GetRemoveableColor()
    {
        return Color.gray;
    }

    public TileBluePrint()
    {

    }

    public TileBluePrint(TileBluePrint toCopy)
    {
        type = toCopy.type;
        id = toCopy.id;
    }

    public virtual bool CanPlace(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        return false;
    }
    public virtual bool CanRemove(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        return false;
    }
}
