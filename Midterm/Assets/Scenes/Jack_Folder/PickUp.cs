using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUp : MonoBehaviour
{
    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log("Collision Detected");
        SceneManager.LoadScene(4);
    }
}
