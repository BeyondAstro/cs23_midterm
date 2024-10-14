using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;

public class AiChase : MonoBehaviour
{
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxSpeed = 3.5f;
    [SerializeField] private float knockbackForce = 50f; // Knockback force
    [SerializeField] private int enemyHealth = 4; // Track the number of bullet collisions
    [SerializeField] private float deceleration = 1f; // Deceleration rate
    [SerializeField] private float chaseDelay = 0.75f;


    public GameObject playerGameObject;
    public Transform player;
    private int bulletCollisionCount = 0; // Track the number of bullet collisions
    private float currentSpeed = 1f;
    public float chaseDistance = 10f;
    public float moveSpeed = 2f;
    // public float pathfindingInterval = 0.5f; // Time interval between pathfinding calculations
    // private Pathfinder realPathfinding;
    // private List<Vector2Int> path;
    // private int currentPathIndex;
    // private float pathfindingTimer;
    private Rigidbody2D rb2d; // Reference to the Rigidbody2D component
    private Knockback knockback;
    private bool isKnocked = false;
    private float distance;
    public Tilemap groundTilemap; // Reference to the ground tilemap
    private HashSet<Vector2Int> obstacleCoordTiles;
    private bool canChase = false; // Flag to control when the enemy can start chasing


    void Start()
    {
        obstacleCoordTiles = getObstacleCoordTiles();
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        player = playerGameObject.transform;
        // realPathfinding = GetComponent<Pathfinder>();
        // pathfindingTimer = pathfindingInterval;
        rb2d = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D component
        knockback = GetComponent<Knockback>();
        StartCoroutine(StartChaseAfterDelay());
    }

    private IEnumerator StartChaseAfterDelay()
    {
        yield return new WaitForSeconds(chaseDelay);
        canChase = true;
    }
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.position);

        // Unfinished astar implemention. Will be revisted

        // float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // pathfindingTimer -= Time.deltaTime;
        // if (distanceToPlayer <= chaseDistance && pathfindingTimer <= 0)
        // {
        //     Vector2Int start = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //     Vector2Int end = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.y));
        //     path = realPathfinding.FindPath(start, end, obstacleCoordTiles);
        //     Debug.Log("this is the path");
        //     for (int i = 0; i < path.Count; i++)
        //     {
        //         Debug.Log(path[i]);
        //     }
        //     Debug.Log(path);
        //     currentPathIndex = 0;
        //     pathfindingTimer = pathfindingInterval;
        // }
        // if (path != null && currentPathIndex < path.Count)
        // {
        //     Debug.Log(path.Count); 
        //     Debug.Log(currentPathIndex);
        //     Debug.Log(path[currentPathIndex]);
        //     Vector2 playerPosition = new Vector2(path[currentPathIndex].x, path[currentPathIndex].y);
        //     transform.position = Vector2.MoveTowards(transform.position, playerPosition, moveSpeed * Time.deltaTime);
        //     if (Vector2.Distance(transform.position, playerPosition) < 0.1f)
        //     {
        //         currentPathIndex++;
        //     }
        // }
        if (canChase && !isKnocked && distance <= chaseDistance && HasLineOfSight())
        {
            chase();
        }
        else if (rb2d.velocity.magnitude <= 0.1f) // Check if the enemy has stopped moving
        {
            isKnocked = false;
        }
        else if (isKnocked)
        {
            currentSpeed -= 10 * Time.deltaTime;
            Vector2 direction = -1 * (player.position - transform.position).normalized;
            rb2d.velocity = direction * currentSpeed;
        }
        else
        {
            // Gradually reduce the speed to zero
            currentSpeed = Mathf.Max(0, currentSpeed - deceleration * Time.deltaTime);
            rb2d.velocity = rb2d.velocity.normalized * currentSpeed;
        }
    }

    private bool HasLineOfSight()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        int layerMask = LayerMask.GetMask("Default") & ~LayerMask.GetMask("Ignore Raycast"); // Exclude the "Enemy" layer

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, chaseDistance, layerMask);
        if (hit.collider != null)
        {
            return hit.collider.transform == player;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            isKnocked = true;
            bulletCollisionCount++; // Increment the collision count
            Destroy(collision.gameObject); // Destroy the bullet
            // Apply knockback
            knockback.Kb(collision.transform, knockbackForce);

            if (bulletCollisionCount >= enemyHealth)
            {
                Destroy(gameObject); // Destroy the enemy after 4 collisions
            }
        }
    }

    private void chase()
    {
        if (currentSpeed <= maxSpeed)
        {
            // Continuously increase the speed
            currentSpeed += acceleration * Time.deltaTime;
        }
        // Calculate the direction towards the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move towards the player using Rigidbody2D
        rb2d.velocity = direction * currentSpeed;
    }

    private HashSet<Vector2Int> getObstacleCoordTiles()
    {
        HashSet<Vector2Int> obstacleCoordTiles = new HashSet<Vector2Int>();
        List<Collider2D> colliders = GetCollidersWithTags(new string[] { "Obstacle", "Lava" });

        foreach (Collider2D collider in colliders)
        {
            Bounds colliderBounds = collider.bounds;
            Vector3Int min = groundTilemap.WorldToCell(colliderBounds.min);
            Vector3Int max = groundTilemap.WorldToCell(colliderBounds.max);
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    if (groundTilemap.HasTile(tilePosition))
                    {
                        obstacleCoordTiles.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        obstacleCoordTiles.Remove(new Vector2Int(0, 0));
        return obstacleCoordTiles;
    }
    public static List<Collider2D> GetCollidersWithTags(string[] tags)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        foreach (string tag in tags)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in taggedObjects)
            {
                Collider2D collider = obj.GetComponent<Collider2D>();
                if (collider != null)
                {
                    colliders.Add(collider);
                }
            }
        }
        return colliders;
    }
}