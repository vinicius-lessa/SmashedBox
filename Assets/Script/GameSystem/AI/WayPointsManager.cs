using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class WayPointsManager : MonoBehaviour
{
    public List<Transform> wayPoitsList = new List<Transform>();
    
    public GameObject targetPlayer;  // Where the NPC will Look at
    public GameObject playerBox;

    private Transform targetWaypoint;
        
    private int isWalkingHash;    
    private float timer;
    
    private int npcIndex;
    private int targetWaypointIndex;    
    private int lastTargetWaypointIndex = -1; // only to ignore for the first moment
    private int npcSecondWait;
    private float minDistance = 0.09f;
    private float lastWaypointIndex; 
    

    // # Walking NPCs
    public List<GameObject> WalkingNPCs         = new List<GameObject>();   // NPC's walking
    public List<GameObject> wayPointToSpawn     = new List<GameObject>();   // All WayPoint
    private List<GameObject> npcsChosen         = new List<GameObject>();   // Sorted NPC's

    // # Jogging NPCs
    public List<Transform> JoggingPointsSpawn   = new List<Transform>();    // Jogging Spawn Locations
    public List<GameObject> JoggingNPCs        = new List<GameObject>();    // Sorted Jogging NPC's 
    private int sortNpcSelcted;
    
    private void Start() {
        if (WalkingNPCs != null && wayPointToSpawn != null)
            NpcsWalkingSpawn()  ; // Walking NPCs
        
        if (JoggingNPCs != null && JoggingPointsSpawn != null)
            NpcsJoggingSpawn()  ; // Jogging NPC
    }

    public void NpcsWalkingSpawn () {
        int sortMaxNpcs = Random.Range(2, wayPointToSpawn.Count);

        // Sort dos NPCs selecionados
        for (int i = 0; i < sortMaxNpcs; i++) { // Passará de 1 a 3 vezes            
            sortNpcSelcted = Random.Range(0, WalkingNPCs.Count); // Seleciona qualquer NPC da Lista

            WalkingNPCs[sortNpcSelcted].gameObject.SetActive(true);
            var npcSpawned = WalkingNPCs[sortNpcSelcted].gameObject;
            npcSpawned.LeanMove(wayPointToSpawn[i].transform.position, 0f);

            npcsChosen.Add(npcSpawned);

            WalkingNPCs.Remove(WalkingNPCs[sortNpcSelcted]); // Evita de repetir NPC
        } 
        
        if (npcsChosen.Count != 0) {
            isWalkingHash = Animator.StringToHash("isWalking");

            for (int i = 0; i < npcsChosen.Count; i++) {
                Animator _tmpNpcAnimator = npcsChosen[i].GetComponent<Animator>();
                
                StartCoroutine(NavigationManager(i, _tmpNpcAnimator));            
            }
        } else {
            Debug.LogWarning("Atenção, problema no 'sort' dos NPCs á andar. - NpcsWalkingSpawn() - WayPoitsManager.cs");
        }
    }
    
    private void NpcsJoggingSpawn() {        
        int sortJoggingNpc = Random.Range(0, JoggingNPCs.Count);
        int sortJoggingSpawn = Random.Range(0, JoggingPointsSpawn.Count); 
        var npcSpawned = JoggingNPCs[sortJoggingNpc].gameObject;

        npcSpawned.gameObject.SetActive(true);        
        npcSpawned.gameObject.LeanMove(JoggingPointsSpawn[sortJoggingSpawn].position, 0f);        

        StartCoroutine(StartJoggingMoving(npcSpawned, sortJoggingSpawn));
    }    
    
    IEnumerator StartJoggingMoving (GameObject npcChosen, int indexWayPoint) {
        var npcNavMesh = npcChosen.GetComponent<NavMeshAgent>();
        var wayPointTransform = JoggingPointsSpawn[indexWayPoint];
        float angularSpeedOriginal = npcNavMesh.angularSpeed;

        if (Vector3.Distance(npcChosen.transform.position, wayPointTransform.position) > minDistance){ // Não está no WayPoint - StartsJogging            
            npcNavMesh.SetDestination(wayPointTransform.position);
        }

        while (Vector3.Distance(npcChosen.transform.position, wayPointTransform.position) > npcNavMesh.stoppingDistance) { // Enquanto não chegar, fica no While
            yield return new WaitForSeconds(0.1f);            
        }        
        
        yield return new WaitForSeconds(Random.Range(3f, 6f));

        indexWayPoint = indexWayPoint == 0 ? indexWayPoint = 1 : indexWayPoint = 0; // Muda WayPoint

        yield return StartJoggingMoving(npcChosen, indexWayPoint);
    }

    IEnumerator NavigationManager (int index, Animator tmpAnimator) {
        NavMeshAgent _tmpNavMeshAgent   = npcsChosen[index].GetComponent<NavMeshAgent>();
        AnimationClip[] clips = tmpAnimator.runtimeAnimatorController.animationClips;

        float speedNormal               = _tmpNavMeshAgent.speed;
        float turningSpeed              = .5f;
        float tuningAnimationLenght     = 0; // Default
        float angularSpeedNpc           = _tmpNavMeshAgent.angularSpeed;        
        // float timer;

        foreach(AnimationClip clip in clips) {            
            if (clip.name == "TurnAround")
                tuningAnimationLenght = clip.length;  // Tempo da animação
        }        

        // Sort NEW WayPoint
        targetWaypointIndex = Random.Range(0,wayPoitsList.Count);            

        // Do not allow to be the same WayPoint that was sorted previously
        while (lastTargetWaypointIndex == targetWaypointIndex) {
            targetWaypointIndex = Random.Range(0,wayPoitsList.Count);
        }
        lastTargetWaypointIndex = targetWaypointIndex; // Update last selected Waypoint
        
        // Transform of Watpoint TMP
        Transform tmpTargetWaypoint = wayPoitsList[targetWaypointIndex];
        
        if (Vector3.Distance(npcsChosen[index].transform.position, tmpTargetWaypoint.position) > minDistance){ // Não está no WayPoint - StartsWalking
            tmpAnimator.SetBool(isWalkingHash, true); // TUNRS -> WALKING ANIMATION Starts
                        
            _tmpNavMeshAgent.speed = turningSpeed; // Turns Animation Start
            _tmpNavMeshAgent.SetDestination(tmpTargetWaypoint.position);            
        }

        if (tmpTargetWaypoint.CompareTag("WayPointLookToPlayer")) { // Look To Player            
            if (npcsChosen[index].transform.GetChild(0).gameObject.GetComponent<Rig>().weight < 1f) {                
                float timer = 0;
                while (timer < 1f) {
                    npcsChosen[index].transform.GetChild(0).gameObject.GetComponent<Rig>().weight = timer;
                    timer = timer + 0.1f;
                    yield return new WaitForSeconds(0.025f);
                }
            }            
        } else { // Stop Looking (only if was looking already)
            if (npcsChosen[index].transform.GetChild(0).gameObject.GetComponent<Rig>().weight > 0f) {                
                float timer = 1;
                while (timer > 0f) {
                    npcsChosen[index].transform.GetChild(0).gameObject.GetComponent<Rig>().weight = timer;
                    timer = timer - 0.1f;
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
                
        yield return new WaitForSeconds(tuningAnimationLenght);        
        _tmpNavMeshAgent.speed = speedNormal; // Turns End

        while (Vector3.Distance(npcsChosen[index].transform.position, tmpTargetWaypoint.position) > _tmpNavMeshAgent.stoppingDistance) { // Enquanto não chegar, fica no While
            yield return new WaitForSeconds(0.1f);            
        }

        _tmpNavMeshAgent.angularSpeed = 0; // Trava angular speed

        tmpAnimator.SetBool(isWalkingHash, false); // IDLE ANIMATION Starts
        
        npcSecondWait = Random.Range(3, 11); // Random Seconds to wait in Waypoint

        //Debug.Log("Waiting for " + npcSecondWait + " seconds");
        yield return new WaitForSeconds(npcSecondWait);

        _tmpNavMeshAgent.angularSpeed = angularSpeedNpc;

        yield return NavigationManager(index, tmpAnimator);
    }
}