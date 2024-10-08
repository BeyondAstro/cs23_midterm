using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    public Transform target;
    private Vector3 vel = Vector3.zero;

    // References to the border GameObjects
    public BoxCollider2D topBorder;
    public BoxCollider2D bottomBorder;
    public BoxCollider2D leftBorder;
    public BoxCollider2D rightBorder;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for SmoothCameraFollow.");
        }
        if (topBorder == null || bottomBorder == null || leftBorder == null || rightBorder == null)
        {
            Debug.LogError("Borders not set for SmoothCameraFollow.");
        }
    }

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        // Calculate the camera bounds
        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraHalfHeight = Camera.main.orthographicSize;

        float minX = leftBorder.bounds.max.x + cameraHalfWidth;
        float maxX = rightBorder.bounds.min.x - cameraHalfWidth;
        float minY = bottomBorder.bounds.max.y + cameraHalfHeight;
        float maxY = topBorder.bounds.min.y - cameraHalfHeight;

        // Clamp the target position within the bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
