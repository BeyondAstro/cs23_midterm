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
    public GameObject[] player;
    private int bulletCollisionCount = 0; // Track the number of bullet collisions
    private float currentSpeed = 1f;
    private float distance;
    private Rigidbody2D rb2d; // Reference to the Rigidbody2D component
    private Knockback knockback;
    private bool isKnocked = false;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        target = player[0].GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D component
        knockback = GetComponent<Knockback>();
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, target.position);
        if (!isKnocked && distance <= 9f) // Check if the enemy is not knocked back
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
}