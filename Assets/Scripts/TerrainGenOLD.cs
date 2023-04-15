using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenOLD : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase grassTile, stoneTile;
    [SerializeField] int width,worldHeight;
    [SerializeField] int minStoneHeight, maxStoneHeight;
    [SerializeField] int seed;
    [Range(0,100)]
    [SerializeField] float noiseScale, smoothness;
    

    void Start()
    {
        seed = Random.Range(-1000000, 1000000);
        Generate();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            tilemap.ClearAllTiles();
            Generate();
        }
    }

    void Generate()
    {
        for (int x = 0; x < width; x++)// Spawns tile on X and Y axis
        {
            // Noise generation for procedural height
            int height = Mathf.RoundToInt(noiseScale * Mathf.PerlinNoise(x / smoothness, seed));

            int minStoneSpawnDistance = height - minStoneHeight;
            int maxStoneSpawnDistance = height - maxStoneHeight;

            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSpawnDistance);

            for (int y = 0; y < height; y++)
            {
                if (y < totalStoneSpawnDistance)// Spawns stone if it's within stone area
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }
            }      
        }
    }
}
