using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


public class TileAutomata : MonoBehaviour
{
    // 0 - 100
    public int iniChance = 25;
    // 0 - 10
    public int birthLimit = 3;
    // 0 - 10
    public int deathLimit = 2;
    // 0 - 10
    public int numR = 8;


    private int[,] terrainMap;
    public Vector3Int tmpSize;
    public Tilemap topMap;
    public Tilemap botMap;
    public RuleTile spawnAreaTile;
    public RuleTile biome1TopTile;
    public RuleTile biome2TopTile;
    public RuleTile biomeOverlapTopTile;
    public RuleTile botTile;


    int width;
    int height;


    public void doSim(int nu)
    {
        clearMap(false);
        width = tmpSize.x;
        height = tmpSize.y;

        if (terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }
        addRandomDiamonds(Random.Range(20, 25), 8, 25);
        addRandomRectangles(Random.Range(30, 40), 6, 25, 6, 25);
        replaceClustersNearCenter();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), biome1TopTile);
                else if (terrainMap[x, y] == 2)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), biome2TopTile);
                else if (terrainMap[x, y] == 3)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), biomeOverlapTopTile);
                else if (terrainMap[x, y] == 4)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), spawnAreaTile);
                else
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
            }
        }
    }
    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = 0;
            }
        }
    }

    public void replaceClustersNearCenter()
    {
        int centerX = width / 2;
        int centerY = height / 2;
        int radius = 25;
        // Randomly select a radius for the circle
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int nx = centerX + x;
                int ny = centerY + y;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && x * x + y * y <= radius * radius)
                {
                    terrainMap[nx, ny] = 4;
                }
            }
        }
    }

    public void clearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }

    public void addRandomDiamonds(int numberOfDiamonds, int minSize, int maxSize)
    {
        for (int i = 0; i < numberOfDiamonds; i++)
        {
            // Randomly select a position on the map
            int centerX = Random.Range(0, width);
            int centerY = Random.Range(0, height);

            // Randomly select a size for the diamond
            int size = Random.Range(minSize, maxSize);

            // Fill a diamond-shaped area with 1s
            for (int x = -size; x <= size; x++)
            {
                for (int y = -size; y <= size; y++)
                {
                    int nx = centerX + x;
                    int ny = centerY + y;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && Mathf.Abs(x) + Mathf.Abs(y) <= size)
                    {
                        terrainMap[nx, ny] = 1;
                    }
                }
            }
        }
    }
    public void addRandomRectangles(int numberOfRectangles, int minWidth, int maxWidth, int minHeight, int maxHeight)
    {
        for (int i = 0; i < numberOfRectangles; i++)
        {
            // Randomly select a position on the map
            int startX = Random.Range(0, width);
            int startY = Random.Range(0, height);

            // Randomly select dimensions for the rectangle
            int rectWidth = Random.Range(minWidth, maxWidth);
            int rectHeight = Random.Range(minHeight, maxHeight);

            // Fill a rectangular area with 1s
            for (int x = 0; x < rectWidth; x++)
            {
                for (int y = 0; y < rectHeight; y++)
                {
                    int nx = startX + x;
                    int ny = startY + y;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        if (terrainMap[nx, ny] == 1)
                        {
                            terrainMap[nx, ny] = 3;
                        }
                        else
                        {
                            terrainMap[nx, ny] = 2;
                        }

                    }
                }
            }
        }
    }
    void Start()
    {
        doSim(numR);
    }
}
