using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractDoor : MonoBehaviour
{
      public string NextLevel = "MainMenu";
      public GameObject msgPressE;
      public bool canPressE = false;
      public float msgDistanceThreshold = 3f; // Distance threshold for enabling interaction
      public float exitDistanceThreshold = 0.5f; // Distance threshold for entering the door
      private GameObject player;
      public GameObject exitPoint;
      public bool useObjective = false;
      public ExitObjective exitObjective = null; // Reference to the ExitObjective component


      void Start()
      {
            msgPressE.SetActive(false);
            player = GameObject.FindGameObjectWithTag("Player");
      }

      void Update()
      {
            if (player != null)
            {
                  if ((useObjective == true && exitObjective.objectiveComplete))
                  {
                        float distance = Vector2.Distance(exitPoint.transform.position, player.transform.position);
                        if (distance <= msgDistanceThreshold)
                        {
                              msgPressE.SetActive(true);
                              if (distance <= exitDistanceThreshold && Input.GetKeyDown(KeyCode.E))
                              {
                                    EnterDoor();
                              }
                        }
                        else
                        {
                              msgPressE.SetActive(false);

                        }
                  }
                  else if (useObjective == false)
                  {
                        float distance = Vector2.Distance(exitPoint.transform.position, player.transform.position);
                        if (distance <= msgDistanceThreshold)
                        {
                              msgPressE.SetActive(true);
                              if (distance <= exitDistanceThreshold && Input.GetKeyDown(KeyCode.E))
                              {
                                    EnterDoor();
                              }
                        }
                        else
                        {
                              msgPressE.SetActive(false);

                        }
                  }
            }
      }

      public void EnterDoor()
      {
            SceneManager.LoadScene(NextLevel);
      }

      void OnTriggerEnter2D(Collider2D other)
      {
            if (other.gameObject.tag == "Player")
            {
                  msgPressE.SetActive(true);
                  canPressE = true;
            }
      }

      void OnTriggerExit2D(Collider2D other)
      {
            if (other.gameObject.tag == "Player")
            {
                  msgPressE.SetActive(false);
                  canPressE = false;
            }
      }
}