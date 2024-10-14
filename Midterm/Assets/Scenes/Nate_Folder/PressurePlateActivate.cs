using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateActivate : MonoBehaviour
{
    public float successDistanceThreshold = 1f;
    public GameObject[] objectivePoints;
    public GameObject[] objectives;
    public bool objectiveComplete = false;
    public GameObject blocker;
    // Start is called before the first frame update

    // Update is called once per frame
    void Start()
    {
        blocker.SetActive(true);
    }
    void Update()
    {
        if (objectiveComplete == false && CheckObjectiveCompletion())
        {
            blocker.SetActive(false);
            objectiveComplete = true;
        }
    }

    private bool CheckObjectiveCompletion()
    {
        for (int i = 0; i < objectives.Length; i++)
        {

            float distance = Vector3.Distance(objectivePoints[i].transform.position, objectives[i].transform.position);
            if (Vector3.Distance(objectivePoints[i].transform.position, objectives[i].transform.position) >= successDistanceThreshold)
            {
                return false;
            }
        }
        return true;
    }
}
