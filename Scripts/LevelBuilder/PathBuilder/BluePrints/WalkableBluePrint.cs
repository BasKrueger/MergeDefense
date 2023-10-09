using System.Collections.Generic;
using UnityEngine;

public class WalkableBluePrint : TileBluePrint
{
    public override Color GetPlaceableColor()
    {
        return Color.white;
    }
    public override Color GetRemoveableColor()
    {
        return Color.red;
    }
    public override Color GetPlacedColor()
    {
        return Color.green;
    }

    public override bool CanPlace(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        if (value.type != TileType.None)
        {
            return false;
        }

        List<TileBluePrint> allPlacedWalkableBluePrints = placedTiles.Filter(TileType.Walkable);

        if (allPlacedWalkableBluePrints.Count == 0)
        {
            if (value.id.x == 0)
            {
                return true;
            }
            return false;
        }

        TileBluePrint lastTile = allPlacedWalkableBluePrints[allPlacedWalkableBluePrints.Count - 1];
        if (Vector3.Distance(value.id, lastTile.id) > 1)
        {
            return false;
        }

        return true;
    }

    public override bool CanRemove(TileBluePrint value, List<TileBluePrint> placedTiles)
    {
        List<TileBluePrint> allPlacedWalkableBluePrints = placedTiles.Filter(TileType.Walkable);

        if (allPlacedWalkableBluePrints.Count == 0)
        {
            return false;
        }

        if (allPlacedWalkableBluePrints[allPlacedWalkableBluePrints.Count - 1].id != value.id)
        {
            return false;
        }

        return true;
    }
}
