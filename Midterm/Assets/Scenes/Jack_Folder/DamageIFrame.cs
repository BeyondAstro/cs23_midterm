using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIFrame : MonoBehaviour
{
    public TMP_Text healthText;
    public GameObject model1;
    public GameObject model2;
    public GameObject player;
    int health = 5;
    bool Invincible = false;
    float IFrameTime = 1.5f;
    float invincibilityDeltaTime = 0.15f;

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log("Collision Detected");
        if (c.gameObject.CompareTag("Enemy") && !Invincible) {
            health--;
            updateHealth();
            if (health == 0) player.SetActive(false);
            StartCoroutine(BecomeTemporarilyInvincible());
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        Invincible = true;
        Debug.Log("Player Invincible");
        for (float i = 0; i < IFrameTime; i += invincibilityDeltaTime)
        {
            model1.SetActive(false);
            model2.SetActive(false);
            yield return new WaitForSeconds(invincibilityDeltaTime);
            model1.SetActive(true);
            model2.SetActive(true);
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        model1.SetActive(true);
        model2.SetActive(true);
        Invincible = false;
        Debug.Log("Player no longer Invincible");
    }

    private void updateHealth() {
        healthText.SetText("Health: " + health);
    }
}
