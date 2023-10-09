using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableBluePrint : TileBluePrint
{
    public override Color GetPlaceableColor()
    {
        return Color.white;
    }
    public override Color GetRemoveableColor()
    {
        return Color.blue;
    }
    public override Color GetPlacedColor()
    {
        return Color.blue;
    }

    public override bool CanPlace(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        if (value.type != TileType.None)
        {
            return false;
        }

        return true;
    }
    public override bool CanRemove(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        return value.type == TileType.Buildable;
    }
}
