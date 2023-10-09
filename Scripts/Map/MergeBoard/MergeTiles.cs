using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTiles : MonoBehaviour
{
    [SerializeField]
    private List<Color> errorBlinkColors = new List<Color>();

    private List<MergeTile> tiles;

    private void Awake()
    {
        tiles = new List<MergeTile>();
        foreach (MergeTile tile in GetComponentsInChildren<MergeTile>())
        {
            tiles.Add(tile);
        }
    }

    public void SetUp()
    {
        foreach(MergeTile tile in tiles)
        {
            tile.SetUp();
        }
    }

    public bool CanSpawnTurret()
    {
        foreach(MergeTile tile in tiles)
        {
            if (tile.CanRegisterTurret())
            {
                return true;
            }
        }

        foreach (MergeTile tile in tiles)
        {
            tile.StartBlinking(errorBlinkColors, 0.5f,0.3f,true);
        }

        return false;
    }

    public bool TrySpawnTurret(Turret template, int level)
    {
        foreach (MergeTile tile in tiles)
        {
            if(tile.CanRegisterTurret())
            {
                Turret active = Instantiate(template);
                active.SetUp(level);
                tile.RegisterTurret(active);
                return true;
            }
        }

        return false;
    }
}
