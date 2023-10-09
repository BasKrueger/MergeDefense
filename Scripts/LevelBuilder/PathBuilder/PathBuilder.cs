#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PathBuilder
{
    public const int width = 8;
    public const int height = 13;

    [ShowInInspector, HideLabel, EnumToggleButtons]
    private BuildMode mode;

    [DisableInPlayMode]
    [ShowInInspector, TableMatrix(HorizontalTitle = "Map Layout", DrawElementMethod = "UpdateTile", RowHeight = 16, SquareCells = true, Transpose = true)]
    private TileBluePrint[,] BluePrintTable = new TileBluePrint[height, width];
    public List<TileBluePrint> placedBluePrints { get; private set; } = new List<TileBluePrint>();

    public bool IsPathValid()
    {
        List<TileBluePrint> moveTiles = GetPlacedBluePrints(TileType.Walkable);
        if (moveTiles == null || moveTiles.Count == 0)
        {
            return false;
        }

        if (moveTiles[moveTiles.Count - 1].id.x != height - 1)
        {
            return false;
        }

        return true;
    }
    public List<TileBluePrint> GetPlacedBluePrints(TileType type)
    {
        return placedBluePrints.Filter(type);
    }
    public void LoadPath(LevelData data)
    {
        placedBluePrints = data.placedBluePrints.Clone();
        ReFillTable();

        for(int x =0; x < BluePrintTable.GetLength(0); x++)
        {
            for(int y =0; y < BluePrintTable.GetLength(1); y++)
            {
                foreach(TileBluePrint bluePrint in data.placedBluePrints)
                {
                    if(bluePrint.id.x == x && bluePrint.id.y == y)
                    {
                        BluePrintTable[x, y].type = bluePrint.type;
                    }
                }
            }
        }
    }

    public void ReFillTable()
    {
        BluePrintTable = new TileBluePrint[height, width];

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                BluePrintTable[x, y] = new TileBluePrint();
                BluePrintTable[x, y].id = new Vector2(x, y);

                if(placedBluePrints != null)
                {
                    foreach (TileBluePrint tile in placedBluePrints)
                    {
                        if (tile.id == BluePrintTable[x, y].id)
                        {
                            BluePrintTable[x, y].type = tile.type;
                            GUI.changed = true;
                        }
                    }
                }
            }
        }

    }

    private TileBluePrint UpdateTile(Rect rect, TileBluePrint value)
    {
        if(value == null)
        {
            ReFillTable();
            return new TileBluePrint();
        }

        TileBluePrint TileClass = mode.ToBluePrintClass();

        Color drawColor = FindPlacedTile(value.id, value.type) == null ? Color.gray : value.type.ToBluePrint().GetPlacedColor();

        if(TileClass.CanPlace(value, placedBluePrints)) { drawColor = TileClass.GetPlaceableColor(); }
        if(TileClass.CanRemove(value, placedBluePrints)) { drawColor = TileClass.GetRemoveableColor(); }

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (TileClass.CanPlace(value, placedBluePrints))
            {
                value.type = mode.ToTileType();
                AddPlacedTile(value.id, value.type);
            }
            else if (TileClass.CanRemove(value, placedBluePrints))
            {
                value.type = TileType.None;
                RemovePlacedTileAt(value.id);
            }
        }
        UnityEditor.EditorGUI.DrawRect(rect.Padding(1), new Color(drawColor.r, drawColor.g, drawColor.b, 0.5f));
        return value;

        #region support functions
        void RemovePlacedTileAt(Vector2 id)
        {
            List<TileBluePrint> toRemove = new List<TileBluePrint>();
            foreach (TileBluePrint bluePrint in placedBluePrints)
            {
                if (bluePrint.id == id)
                {
                    toRemove.Add(bluePrint);
                }
            }

            foreach (TileBluePrint bluePrint in toRemove)
            {
                placedBluePrints.Remove(bluePrint);
            }
        }
        void AddPlacedTile(Vector2 id, TileType type)
        {
            foreach (TileBluePrint bluePrint in placedBluePrints)
            {
                if (bluePrint.id == id)
                {
                    bluePrint.type = type;
                    return;
                }
            }

            TileBluePrint toAdd = new TileBluePrint();
            toAdd.id = id;
            toAdd.type = type;
            placedBluePrints.Add(toAdd);
        }
        TileBluePrint FindPlacedTile(Vector2 id, TileType type)
        {
            if(placedBluePrints == null)
            {
                return null;
            }

            foreach(TileBluePrint bluePrint in placedBluePrints.Filter(type))
            {
                if(bluePrint.id == id)
                {
                    return bluePrint;
                }
            }

            return null;
        }
        #endregion
    }

    [Button(ButtonSizes.Gigantic, Icon = SdfIconType.Trash, Name = "Clear")]
    public void ClearButton()
    {
        Clear();
    }

    public void Clear(bool skipConfirmation = false)
    {
        if(!skipConfirmation && placedBluePrints.Count > 0)
        {
            if(!EditorUtility.DisplayDialog("Levelbuilder", "Are you sure that you want to clear the levellayout?", "Yes", "No"))
            {
                return;
            }
        }

        BluePrintTable = new TileBluePrint[height, width];
        placedBluePrints = new List<TileBluePrint>();
        ReFillTable();
    }
}
#endif