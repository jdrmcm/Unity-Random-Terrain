using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGen : MonoBehaviour
{
    [Header("Tiles")]

    [SerializeField] Tilemap groundTilemap, backgroundTilemap;
    [SerializeField] TileBase grassTile, stoneTile, treeTile, leafTile;

    [Header("Terrain Settings")]
    [SerializeField] int worldSize = 100;
    [SerializeField] int grassLayerHeight = 5;

    [SerializeField] float surfaceValue = 0.25f;
    [SerializeField] float caveFreq = 0.05f;
    [SerializeField] float terrainFreq = 0.05f;
    [SerializeField] int heightAddition = 25;
    [SerializeField] float heightMultiplier = 4f;
    [SerializeField] float seed;

    [Header("Generator Settings")]
    [SerializeField] int chunkSize = 16;
    [SerializeField] bool generateCaves;
    [SerializeField] bool generateTrees;
    [SerializeField] int treeChance = 10;

    Texture2D noiseTexture;

    List<TileBase> backgroundTiles = new List<TileBase>();
    List<TileBase> groundTiles = new List<TileBase>();

    GameObject[] worldChunks;

    void Start()
    {
        backgroundTiles.Add(treeTile);
        backgroundTiles.Add(leafTile);

        groundTiles.Add(grassTile);
        groundTiles.Add(stoneTile);

        seed = Random.Range(-10000000, 10000000);
        GenerateNoise();
        CreateChunks();
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            // Convoluted way to create noise for height variation
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;

            for (int y = 0; y < height; y++)
            {
                if (generateCaves)
                {
                    if (noiseTexture.GetPixel(x, y).r > surfaceValue)
                    {
                        PlaceTerrain(x, y, height);
                    }
                }
                else
                {
                    PlaceTerrain(x, y, height);
                }
            }
        }
    }

    void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }

    // Generates noise texture used by terrain gen
    void GenerateNoise()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }

    void PlaceTerrain(int x, int y, float height)
    {
        // Sets target tile to place to stone or grass depending on just layer height
        TileBase tile;

        if (y < height - grassLayerHeight)
        {
            tile = stoneTile;
        }
        else
        {
            tile = grassTile;
        }
        if (y >= height - 1)
        {
            GenerateObjects(x, y, height);
        }
        PlaceTile(x, y, tile);
    }

    void GenerateObjects(int x, int y, float height)
    {
        if (generateTrees)
        {
            int tree = Random.Range(0, treeChance);

            // Creates trees (I know the leaf generation is stupid rn)
            if (tree == 1)
            {
                int treeHeight = Random.Range(4, 10);
                for (int i = 0; i < treeHeight; i++)
                {
                    backgroundTilemap.SetTile(new Vector3Int(x, y + i), treeTile);
                }
                PlaceTile(x, y + treeHeight, leafTile);
                PlaceTile(x + 1, y + treeHeight, leafTile);
                PlaceTile(x - 1, y + treeHeight, leafTile);

                PlaceTile(x, y + treeHeight + 1, leafTile);
                PlaceTile(x + 1, y + treeHeight + 1, leafTile);
                PlaceTile(x - 1, y + treeHeight + 1, leafTile);

                PlaceTile(x, y + treeHeight + 2, leafTile);
            }
        }
    }

    //convert to gameobjects instead of tiles...
    void PlaceTile(int x, int y, TileBase tile)
    {

        int chunkCoord = Mathf.RoundToInt(x / chunkSize) * chunkSize;
        chunkSize /= chunkSize;

        if (backgroundTiles.Contains(tile))
        {
            backgroundTilemap.SetTile(new Vector3Int(x, y), tile);
        }
        else
        {
            groundTilemap.SetTile(new Vector3Int(x, y), tile);
        }
    }
}