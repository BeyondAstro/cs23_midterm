using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTileCollide : MonoBehaviour
{
    public GameObject model;
    public int numShard = 0;
    public int maxShard;

    void Update() {
        if (numShard == maxShard) {
            model.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Trigger");
        if (other.gameObject.tag == "CrateStar") {
            numShard++;
        }
    }
}
