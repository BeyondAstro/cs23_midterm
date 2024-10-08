using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChase : MonoBehaviour
{
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxSpeed = 3.5f;
    [SerializeField] private Transform target;
    [SerializeField] private float knockbackForce = 50f; // Knockback force
    [SerializeField] private int enemyHealth = 4; // Track the number of bullet collisions
    public GameObject[] players;
    public Transform player;
    private int bulletCollisionCount = 0; // Track the number of bullet collisions
    private float currentSpeed = 1f;
    public float chaseDistance = 10f;
    public float moveSpeed = 2f;
    public float pathfindingInterval = 0.5f; // Time interval between pathfinding calculations
    private AStarPathfinding pathfinding;
    private List<Vector2Int> path;
    private int currentPathIndex;
    private float pathfindingTimer;
    private Rigidbody2D rb2d; // Reference to the Rigidbody2D component
    private Knockback knockback;
    private bool isKnocked = false;
    private float distance;
    public BoxCollider2D topBorder;
    public BoxCollider2D bottomBorder;
    public BoxCollider2D leftBorder;
    public BoxCollider2D rightBorder;
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        target = players[0].GetComponent<Transform>();
        player = target;
        pathfinding = GetComponent<AStarPathfinding>();
        rb2d = GetComponent<Rigidbody2D>();
        pathfindingTimer = pathfindingInterval;
        rb2d = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D component
        knockback = GetComponent<Knockback>();
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, target.position);

        // float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // pathfindingTimer -= Time.deltaTime;

        // if (distanceToPlayer <= chaseDistance && pathfindingTimer <= 0)
        // {
        //     Vector2Int start = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //     Vector2Int end = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.y));
        //     bool[,] walkableMap = GenerateWalkableMap(); // Implement this method to generate the walkable map
        //     path = pathfinding.FindPath(start, end, walkableMap);
        //     currentPathIndex = 0;
        //     pathfindingTimer = pathfindingInterval;
        // }
        // if (path != null && currentPathIndex < path.Count)
        // {
        //     Vector2 targetPosition = new Vector2(path[currentPathIndex].x, path[currentPathIndex].y);
        //     transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //     if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        //     {
        //         currentPathIndex++;
        //     }
        // }
        if (!isKnocked && distance <= chaseDistance)
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
            Vector2 direction = -1 * (target.position - transform.position).normalized;
            rb2d.velocity = direction * currentSpeed;
        }
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
        Vector2 direction = (target.position - transform.position).normalized;

        // Move towards the player using Rigidbody2D
        rb2d.velocity = direction * currentSpeed;
    }

    private bool[,] GenerateWalkableMap()
    {
        Collider2D collider = this.GetComponent<Collider2D>();
        float xColliderBounds = collider.bounds.size.x;
        float yColliderBounds = collider.bounds.size.y;
        float totalX = rightBorder.bounds.min.x - leftBorder.bounds.max.x;
        float totalY = topBorder.bounds.min.y - bottomBorder.bounds.max.y;

        // Implement this method to generate the walkable map based on your game's terrain
        // For example, you can use a 2D array where true represents walkable tiles and false represents obstacles
        bool[,] walkableMap = new bool[(int)(totalX / xColliderBounds), (int)(totalY / yColliderBounds)];
        for(int xInd = 0; xInd < walkableMap.GetLength(0)-1; xInd++){
            for(int yInd = 0; yInd < walkableMap.GetLength(1)-1; yInd++){
                walkableMap[xInd, yInd] = rangeIsClear(xInd, yInd, xColliderBounds, yColliderBounds, leftBorder.bounds.max.x, topBorder.bounds.min.y);
            }
        }
        return walkableMap; // Placeholder
    }
    private bool rangeIsClear(int xInd, int yInd, float xRange, float yRange, float leftStartPos, float topStartPos){
        float xStart = leftStartPos + xInd * xRange;
        float xStop = xStart + xRange;
        float yStart = topStartPos - yInd * yRange;
        float yStop = yStart - yRange;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2(xStart, yStart), new Vector2(xStop, yStop), 0);
        return colliders.Length == 0;
    }
}