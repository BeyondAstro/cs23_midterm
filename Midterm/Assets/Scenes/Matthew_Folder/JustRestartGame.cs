using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JustRestartGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void resetGame()
    {
        SceneManager.LoadScene("level1");
    }
}
