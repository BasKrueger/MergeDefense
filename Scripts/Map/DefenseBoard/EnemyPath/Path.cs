using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Path : MonoBehaviour
{
    private const float tileSize = 0.55f;

    [HideInInspector]
    public UnityEvent WaveCompleted;

    [SerializeField]
    private Transform tileHolder;
    private PathEnemies enemies;

    [FoldoutGroup("References"), Required, AssetsOnly]
    public BoardTile emptyTileTemplate;
    [FoldoutGroup("References"), Required, AssetsOnly]
    public WalkableTile walkableTileTemplate;
    [FoldoutGroup("References"), Required, AssetsOnly]
    public BuildTile buildableTileTemplate;

    [HideInInspector]
    private List<BoardTile> spawnedEmptyTiles;
    [HideInInspector]
    public List<WalkableTile> spawnedWalkTiles;
    [HideInInspector]
    private List<BuildTile> spawnBuildTiles;

    private void Awake()
    {
        enemies = GetComponentInChildren<PathEnemies>();
    }

    public void SetUp(LevelData level)
    {
        SpawnLevelLayout(level);

        enemies.SetUp(spawnedWalkTiles[0], level);
        enemies.WaveCompleted.AddListener(OnEnemyWaveCompleted);

        void OnEnemyWaveCompleted()
        {
            WaveCompleted?.Invoke();
        }
    }

    public void StartNextWave()
    {
        enemies.StartNextWave();
    }

    public void SpawnLevelLayout(LevelData level)
    {
        if (level == null)
        {
            return;
        }

        spawnedEmptyTiles = new List<BoardTile>();
        spawnedWalkTiles = new List<WalkableTile>();
        spawnBuildTiles = new List<BuildTile>();

        while (tileHolder.transform.childCount > 0)
        {
            DestroyImmediate(tileHolder.GetChild(0).gameObject);
        }

        Vector3 startPosition = transform.position;
        startPosition.x -= level.width / 2f * tileSize;
        startPosition.z += level.height / 2f * tileSize;

        for (float x = 0; x < level.width; x++)
        {
            for (float z = 0; z < level.height; z++)
            {
                Vector3 spawnPosition = startPosition;
                spawnPosition.x += (x + tileSize) * tileSize;
                spawnPosition.z -= (z + tileSize) * tileSize;

                SpawnTile(new Vector2(z, x), spawnPosition);
            }
        }

        SetUpWalkableTiles();

        void SpawnTile(Vector2 indexPosition, Vector3 spawnPosition)
        {
            if (level == null)
            {
                return;
            }

            if (!TryToSpawnWalkable())
            {
                if (!TryToSpawnBuildable())
                {
                    SpawnEmpty();
                }
            }

            bool TryToSpawnWalkable()
            {
                foreach (TileBluePrint tile in level.placedBluePrints.Filter(TileType.Walkable))
                {
                    if (tile.id == indexPosition)
                    {
                        WalkableTile walkTile = (WalkableTile)GenerateTile(walkableTileTemplate);
                        spawnedWalkTiles.Add(walkTile);
                        return true;
                    }
                }

                return false;
            }
            bool TryToSpawnBuildable()
            {
                foreach (TileBluePrint tile in level.placedBluePrints.Filter(TileType.Buildable))
                {
                    if (tile.id == indexPosition)
                    {
                        BuildTile buildTile = (BuildTile)GenerateTile(buildableTileTemplate);
                        spawnBuildTiles.Add(buildTile);
                        return true;
                    }
                }

                return false;
            }

            void SpawnEmpty()
            {
                spawnedEmptyTiles.Add(GenerateTile(emptyTileTemplate));
            }

            BoardTile GenerateTile(BoardTile toSpawn)
            {
                BoardTile tile = Instantiate(toSpawn);

                tile.gameObject.name = indexPosition.ToString();
                tile.transform.position = spawnPosition;

                tile.transform.SetParent(tileHolder);

                tile.SetUp(indexPosition);

                return tile;
            }
        }
        void SetUpWalkableTiles()
        {
            if (level == null)
            {
                return;
            }

            List<TileBluePrint> unsorted = level.placedBluePrints.Filter(TileType.Walkable);
            List<WalkableTile> sorted = new List<WalkableTile>();

            for (int i = 0; i < unsorted.Count - 1; i++)
            {
                foreach (WalkableTile tile in spawnedWalkTiles)
                {
                    if (tile.Is(unsorted[i].id))
                    {
                        tile.SetUpWalkable(FindTile(unsorted[i + 1].id));
                        sorted.Add(tile);
                    }
                }
            }

            spawnedWalkTiles = sorted;

            WalkableTile FindTile(Vector2 id)
            {
                foreach (WalkableTile tile in spawnedWalkTiles)
                {
                    if (tile.Is(id))
                    {
                        return tile;
                    }
                }

                return null;
            }
        }
    }
}
