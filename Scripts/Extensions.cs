using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static List<TileBluePrint> Filter(this List<TileBluePrint> list, TileType type)
    {
        if (list == null)
        {
            return new List<TileBluePrint>();
        }

        List<TileBluePrint> bluePrints = new List<TileBluePrint>();
        foreach (TileBluePrint bluePrint in list)
        {
            if (bluePrint.type == type)
            {
                bluePrints.Add(bluePrint);
            }
        }
        return bluePrints;
    }

    public static TileBluePrint ToBluePrintClass(this BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Walkable:
                return new WalkableBluePrint();
            case BuildMode.Buildable:
                return new BuildableBluePrint();
        }

        throw new System.Exception("ToBluePrintClass Extension for BuildMode " + mode.ToString() + " not Implemented");
    }

    public static TileBluePrint ToBluePrint(this TileType type)
    {
        switch (type)
        {
            case TileType.Walkable:
                return new WalkableBluePrint();
            case TileType.Buildable:
                return new BuildableBluePrint();
        }

        throw new System.Exception("ToBluePrintClass Extension for TileType " + type.ToString() + " not Implemented");
    }

    public static TileType ToTileType(this BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Walkable:
                return TileType.Walkable;
            case BuildMode.Buildable:
                return TileType.Buildable;
        }

        throw new System.Exception("ToTileType Extension for BuildMode " + mode.ToString() + " not Implemented");
    }

    public static List<WaveData> Clone(this List<WaveData> original)
    {
        List<WaveData> copy = new List<WaveData>();
        foreach (WaveData waveData in original)
        {
            copy.Add(new WaveData(waveData));
        }
        return copy;
    }

    public static List<TileBluePrint> Clone(this List<TileBluePrint> original)
    {
        List<TileBluePrint> copy = new List<TileBluePrint>();

        foreach (TileBluePrint bluePrint in original)
        {
            copy.Add(new TileBluePrint(bluePrint));
        }

        return copy;
    }
}
