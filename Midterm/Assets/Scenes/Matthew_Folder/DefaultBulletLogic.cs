using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBulletLogic : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(1, 10)]
    [SerializeField] private float speed = 2f;

    [Range(1, 10)]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private string ignoreCollisionTag = "IgnoreBulletCollisions"; // Tag to identify objects to ignore

    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        // Find all game objects with the specified tag
        GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(ignoreCollisionTag);
        foreach (GameObject obj in objectsToIgnore)
        {
            Collider2D objCollider = obj.GetComponent<Collider2D>();
            if (objCollider != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), objCollider);
            }
        }
    }

    private void Update()
    {
        rb2d.velocity = transform.up * speed;
    }
}