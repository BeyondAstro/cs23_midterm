using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
        }
    }

    void Update()
    {
        FlipBasedOnRotation();
    }

    private void FlipBasedOnRotation()
    {
        float rotationZ = transform.rotation.eulerAngles.z;

        // Normalize the rotation to the range of -180 to 180 degrees
        if (rotationZ > 180)
        {
            rotationZ -= 360;
        }

        // Flip the sprite based on the rotation
        if (rotationZ > 90 || rotationZ < -90)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }
}
