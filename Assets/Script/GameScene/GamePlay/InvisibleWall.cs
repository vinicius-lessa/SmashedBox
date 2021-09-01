using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {        
        // Igonres PlankWoodPieces
        if (collision.gameObject.CompareTag("PlankWoodPieces")){
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }
    }
}