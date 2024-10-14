using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUp : MonoBehaviour
{
    private static string name = "Scenes/ToBeContinued";

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log("Collision Detected");
        Debug.Log(name);
        SceneManager.LoadScene(SceneManager.GetSceneByName(name).buildIndex);
        Debug.Log(name);
    }
}
