using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int chunkSize = 16;  // Size of each chunk (e.g., 16x16 tiles)
    public float noiseScale = 10f;  // Noise scale for terrain generation
    public GameObject tilePrefab;  // Prefab for the tile
    public Transform player;  // Reference to the player to track movement

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();  // Stores active chunks
    private Vector2Int previousPlayerChunk;  // To detect when the player moves to a new chunk

    void Start()
    {
        UpdateChunks();
    }

    void Update()
    {
        // Track the chunk the player is currently in
        Vector2Int playerChunk = GetChunkFromPosition(player.position);
        
        // Only update chunks if the player has moved to a new chunk
        if (playerChunk != previousPlayerChunk)
        {
            UpdateChunks();
            previousPlayerChunk = playerChunk;
        }
    }

    // Function to update chunks based on player's current chunk
    void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkFromPosition(player.position);

        // Define the radius of chunks to load around the player
        int loadRadius = 2;

        // Load new chunks around the player
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);
                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                }
            }
        }

        // Unload far away chunks
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in activeChunks.Keys)
        {
            if (Vector2Int.Distance(chunk, playerChunk) > loadRadius)
            {
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunkCoord in chunksToRemove)
        {
            UnloadChunk(chunkCoord);
        }
    }

    // Convert world position to chunk coordinate
    Vector2Int GetChunkFromPosition(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkSize);
        int chunkY = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }

    // Generate a chunk at a specific coordinate
    void GenerateChunk(Vector2Int chunkCoord)
    {
        GameObject chunk = new GameObject("Chunk " + chunkCoord);
        chunk.transform.position = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
        
        // Generate tiles for the chunk
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePosition = new Vector3(chunkCoord.x * chunkSize + x, chunkCoord.y * chunkSize + y, 0);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, chunk.transform);

                // Generate terrain using Perlin Noise
                float perlinValue = Mathf.PerlinNoise((tilePosition.x) / noiseScale, (tilePosition.y) / noiseScale);
                
                // Set tile color or type based on Perlin noise
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (perlinValue < 0.4f)
                {
                    sr.color = Color.blue;  // Water
                }
                else if (perlinValue < 0.7f)
                {
                    sr.color = Color.green;  // Grass
                }
                else
                {
                    sr.color = Color.gray;  // Mountains
                }
            }
        }

        activeChunks.Add(chunkCoord, chunk);
    }

    // Unload a chunk (destroy the game objects and remove from the dictionary)
    void UnloadChunk(Vector2Int chunkCoord)
    {
        GameObject chunk;
        if (activeChunks.TryGetValue(chunkCoord, out chunk))
        {
            Destroy(chunk);
            activeChunks.Remove(chunkCoord);
        }
    }
}
