using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankWood : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {        
        // Igonres RockPieces
        if (collision.gameObject.CompareTag("invisibleWall") ^ collision.gameObject.CompareTag("invisibleWallTwo")){            
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }
    }
}