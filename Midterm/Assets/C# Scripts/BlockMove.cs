using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CrateMove : MonoBehaviour {

    void OnCollisionExit2D(Collision2D colExt){
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if ((colExt.gameObject.GetComponent<Rigidbody2D>() != null) && (colExt.gameObject.GetComponent<Rigidbody2D>().bodyType ==RigidbodyType2D.Dynamic)){
            colExt.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }
}
