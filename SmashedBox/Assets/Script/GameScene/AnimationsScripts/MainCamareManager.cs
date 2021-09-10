using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamareManager : MonoBehaviour
{
    public Transform target;
    private Transform defaultPositon;
    private bool isLookingToPLayer;

    private void Start() {
        Time.timeScale = 1f;
        defaultPositon = gameObject.transform;
    }

    public void MoveCameraToPlayer() {
        StartCoroutine(ActionCamera());
    }

    private void Update() {
        if (isLookingToPLayer) {
            transform.LookAt(target);
        }
    }

    public IEnumerator ActionCamera () {
        var orinalPosition = defaultPositon.position;
        var orinalRotation = defaultPositon.rotation;        
        
        float newX = target.transform.GetComponent<Rigidbody>().velocity.x > 0.1f ? target.transform.position.x + 3f : target.transform.position.x - 3f;
        float newY = target.transform.position.y + 1f;
        float newZ = target.transform.position.z - 4.5f;

        var newPosition = new Vector3(newX, newY, newZ);
        
        isLookingToPLayer = true;

        Time.timeScale = .7f;        
        
        this.gameObject.LeanMoveLocal(newPosition, .2f);

        yield return new WaitForSeconds(.5f); // stay looking to the player
                

        // Time.timeScale = .1f;
        this.gameObject.LeanMoveLocal(new Vector3(.1f, 3.5f, -14.1f), .2f);
        yield return new WaitForSeconds(.2f);

        Time.timeScale = 1f;        
        
        isLookingToPLayer = false;
        transform.LookAt(Vector3.zero);
        transform.rotation = orinalRotation;
    }
}