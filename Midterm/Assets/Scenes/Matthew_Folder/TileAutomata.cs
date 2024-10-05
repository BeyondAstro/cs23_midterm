using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class sector
{
    public int xSectorId = -1;
    public int ySectorId = -1;
    public List<Vector3Int> locations = new List<Vector3Int>();
    public List<GameObject> entities = new List<GameObject>();
    public sector(int xSecId, int ySecId, Vector3Int coord, GameObject obj)
    {
        this.xSectorId = xSecId;
        this.ySectorId = ySecId;
        this.entities.Add(obj);
        this.locations.Add(coord);
    }
    public sector(int xSecId, int ySecId)
    {
        this.xSectorId = xSecId;
        this.ySectorId = ySecId;
    }
}
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
    public int obstaclesPerSector = 6;

    private int[,] terrainMap;
    public Vector3Int tmpSize;
    public Tilemap topMap;
    public Tilemap botMap;
    public RuleTile spawnAreaTile;
    public List<sector> sectors = new List<sector>();
    [SerializeField] private GameObject[] spawnAreaObstaclePrefabs;
    public RuleTile biome1TopTile;
    [SerializeField] private GameObject[] biome1ObstaclePrefabs;
    public RuleTile biome2TopTile;
    [SerializeField] private GameObject[] biome2ObstaclePrefabs;
    public RuleTile biomeOverlapTopTile;
    [SerializeField] private GameObject[] biomeOverlapObstaclePrefabs;

    public GameObject player;
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
        addRandomRectangles(Random.Range(40, 50), 6, 25, 6, 25);
        replaceClustersNearCenter();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), biome1TopTile);
                else if (terrainMap[x, y] == 2)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), biome2TopTile);
                else if (terrainMap[x, y] == 4)
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), spawnAreaTile);
                else
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
            }
        }
        SpawnObstaclesStupid();
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
                        terrainMap[nx, ny] = 2;
                    }
                }
            }
        }
    }
    public void SpawnObstaclesStupid()
    {
        HashSet<Vector3Int> spawnPoints = new HashSet<Vector3Int>();
        while (spawnPoints.Count < 500)
        {
            int randomX = Random.Range(-1 * width / 2 + 1, width / 2 - 1);
            int randomY = Random.Range(-1 * height / 2 + 1, height / 2 - 1);
            Vector3Int randomPosition = new Vector3Int(randomX, randomY, 0);
            spawnPoints.Add(randomPosition);
        }
        GameObject obstacle;
        foreach (Vector3Int position in spawnPoints)
        {
            TileBase tile = topMap.GetTile(position);
            if (tile == biome1TopTile)
            {
                obstacle = biome1ObstaclePrefabs[Random.Range(0, biome1ObstaclePrefabs.Length)];
            }
            else if (tile == biome2TopTile)
            {
                obstacle = biome2ObstaclePrefabs[Random.Range(0, biome2ObstaclePrefabs.Length)];
            }
            else if (tile == biomeOverlapTopTile)
            {
                obstacle = biomeOverlapObstaclePrefabs[Random.Range(0, biomeOverlapObstaclePrefabs.Length)];
            }
            else
            {
                obstacle = spawnAreaObstaclePrefabs[Random.Range(0, spawnAreaObstaclePrefabs.Length)];
            }
            GameObject a = Instantiate(obstacle, position, Quaternion.identity);
        }
    }
    public void SpawnObstacles()
    {
        for (int xSec = 0; xSec < width / 5; xSec += 1)
        {
            for (int ySec = 0; ySec < height / 5; ySec += 1)
            {
                sector curSec = new sector(xSec, ySec);
                int x = xSec * 5;
                int y = ySec * 5;
                while (curSec.locations.Count <= obstaclesPerSector)
                {
                    bool isOverlap = false;
                    int randomX = Random.Range(x, x + 5);
                    int randomY = Random.Range(y, y + 5);
                    Vector3Int randomPosition = new Vector3Int(randomX, randomY, 0);
                    foreach (Vector3Int position in curSec.locations)
                    {
                        if (Vector3Int.Distance(randomPosition, position) < 2)
                        {
                            isOverlap = true;
                            break;
                        }
                    }
                    if (isOverlap)
                    {
                        continue;
                    }
                    TileBase tile = topMap.GetTile(randomPosition);
                    GameObject obstacle;
                    if (tile == biome1TopTile)
                    {
                        obstacle = Instantiate(biome1ObstaclePrefabs[Random.Range(0, biome1ObstaclePrefabs.Length)], randomPosition, Quaternion.identity);
                    }
                    else if (tile == biome2TopTile)
                    {
                        obstacle = Instantiate(biome2ObstaclePrefabs[Random.Range(0, biome2ObstaclePrefabs.Length)], randomPosition, Quaternion.identity);
                    }
                    else if (tile == biomeOverlapTopTile)
                    {
                        obstacle = Instantiate(biomeOverlapObstaclePrefabs[Random.Range(0, biomeOverlapObstaclePrefabs.Length)], randomPosition, Quaternion.identity);
                    }
                    else
                    {
                        obstacle = Instantiate(spawnAreaObstaclePrefabs[Random.Range(0, spawnAreaObstaclePrefabs.Length)], randomPosition, Quaternion.identity); ;
                    }
                    GameObject newObstacle = Instantiate(obstacle, randomPosition, Quaternion.identity);
                }
            }
        }
    }
    public Vector2Int GetPlayerSector()
    {
        // Convert the player's world position to a cell position
        Vector3Int playerCellPos = topMap.WorldToCell(player.transform.position);
        // Determine the sector by dividing the cell position by the sector size (5)
        int playerSectorX = playerCellPos.x / 5;
        int playerSectorY = playerCellPos.y / 5;
        return new Vector2Int(playerSectorX, playerSectorY);
    }

    public void InstantiateSectorsAroundPlayer(int n)
    {
        Vector2Int playerSector = GetPlayerSector();
        int playerSectorX = playerSector.x;
        int playerSectorY = playerSector.y;
        foreach (var sector in sectors)
        {
            int sectorX = sector.xSectorId;
            int sectorY = sector.ySectorId;

            if (Mathf.Abs(sectorX - playerSectorX) <= n && Mathf.Abs(sectorY - playerSectorY) <= n)
            {
                foreach (var entity in sector.entities)
                {
                    if (!entity.activeInHierarchy)
                    {
                        entity.SetActive(true);
                    }
                }
            }
            else
            {
                foreach (var entity in sector.entities)
                {
                    if (entity.activeInHierarchy)
                    {
                        entity.SetActive(false);
                    }
                }
            }
        }
    }
    void Start()
    {
        doSim(numR);
    }
    void Update()
    {
        InstantiateSectorsAroundPlayer(1);
    }
}

