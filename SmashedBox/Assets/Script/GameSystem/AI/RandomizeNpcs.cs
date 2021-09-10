using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeNpcs : MonoBehaviour
{
    public List<GameObject> npcsListDefault = new List<GameObject>();  // All NPC's  
    
    public List<Transform> SitPointsToSpawn = new List<Transform>(); // Sitting Spawn Locations
    private List<GameObject> SitNPCToSpawn = new List<GameObject>(); // Sorted Sitting NPC's

    public List<Transform> FishingPointsToSpawn = new List<Transform>(); // Fishing Spawn Locations
    private List<GameObject> FishingNPCToSpawn = new List<GameObject>(); // Sorted Fighisng NPC's

    public List<Transform> FencePointsToSpawn = new List<Transform>(); // Fence Spawn Locations
    private List<GameObject> FenceNPCToSpawn = new List<GameObject>(); // Sorted Fence NPC's
    
    private int sortIndex;
    private int sortMaxNpcs;
    private int sortNpcSelcted;

    
    private void Start() {
        sortMaxNpcs = Random.Range(2, npcsListDefault.Count);        

        // Sort dos NPCs selecionados
        for (int i = 0; i < sortMaxNpcs; i++) {
            sortNpcSelcted = Random.Range(0, npcsListDefault.Count); // Seleciona qualquer NPC da Lista            
            
            // npcsListDefault[sortNpcSelcted].gameObject.SetActive(true);
            
            switch (npcsListDefault[sortNpcSelcted].tag) 
            {
                case "SitNpc":
                    SitNPCToSpawn.Add(npcsListDefault[sortNpcSelcted].gameObject);
                    break;
                case "FishingNpc":                    
                    FishingNPCToSpawn.Add(npcsListDefault[sortNpcSelcted].gameObject);
                    break;
                case "FenceNpc":
                    FenceNPCToSpawn.Add(npcsListDefault[sortNpcSelcted].gameObject);
                    break;                                                        
            }

            npcsListDefault.Remove(npcsListDefault[sortNpcSelcted]); // Evita de repetir NPC
        } 
            
        if (SitNPCToSpawn != null && SitPointsToSpawn != null)
            PostionSittingNpcs();
        if (FishingNPCToSpawn != null && FishingPointsToSpawn != null)            
            PostionFishingNpcs();        
        if (FenceNPCToSpawn != null && FencePointsToSpawn != null)
            PostionFenceNpcs();
    }

    // Spawn Sitting Npcs
    private void PostionSittingNpcs() {        
        for (int i = 0; i < SitNPCToSpawn.Count; i++) {
            if (SitPointsToSpawn != null) {
                int sortIndex = Random.Range(0, SitPointsToSpawn.Count);
                var npcSpawned = SitNPCToSpawn[i].gameObject;
                npcSpawned.gameObject.SetActive(true);                                

                if ((npcSpawned.name == "AdamSitting")){                    
                    var newTransform = new Vector3(SitPointsToSpawn[sortIndex].position.x, SitPointsToSpawn[sortIndex].position.y, SitPointsToSpawn[sortIndex].position.z - .2f);
                    var newRotation = Quaternion.Euler(SitNPCToSpawn[i].transform.rotation.x - 10f, 180f, SitNPCToSpawn[i].transform.rotation.y);

                    SitNPCToSpawn[i].transform.SetPositionAndRotation(newTransform, newRotation); // Seta posição (x,y,z) e rotação (Y)                    
                } else {
                    npcSpawned.LeanMove(SitPointsToSpawn[sortIndex].position, 0f);
                    npcSpawned.LeanRotateY(SitPointsToSpawn[sortIndex].rotation.eulerAngles.y, 0f);
                }
                

                SitPointsToSpawn.Remove(SitPointsToSpawn[sortIndex]); // Evita de Spawnar no mesmo Lugar
            } else {
                Debug.LogWarning("Atenção - há mais NPC's 'SitingNpcs' para spawn do que SpawnPoints. Cancelando Spawn do NPC: " + FenceNPCToSpawn[i].gameObject.name);
                break;
            }
        }
    }

    // Spawn Fishing Npcs
    private void PostionFishingNpcs() {        
        for (int i = 0; i < FishingNPCToSpawn.Count; i++) {            
            if (FishingPointsToSpawn != null) {
                int sortIndex = Random.Range(0, FishingPointsToSpawn.Count);
                var npcSpawned = FishingNPCToSpawn[i].gameObject;
                npcSpawned.gameObject.SetActive(true);                    
                
                npcSpawned.LeanMove(FishingPointsToSpawn[sortIndex].position, 0f);
                npcSpawned.LeanRotateY(FishingPointsToSpawn[sortIndex].rotation.eulerAngles.y, 0f);

                FishingPointsToSpawn.Remove(FishingPointsToSpawn[sortIndex]); // Evita de Spawnar no mesmo Lugar
            } else {
                Debug.LogWarning("Atenção - há mais NPC's 'FishingNpcs' para spawn do que SpawnPoints. Cancelando Spawn do NPC: " + FenceNPCToSpawn[i].gameObject.name);
                break;
            }
        }
    }

    // Spawn Fence Npcs
    private void PostionFenceNpcs() {
        for (int i = 0; i < FenceNPCToSpawn.Count; i++) {            
            if (FencePointsToSpawn != null) {
                int sortIndex = Random.Range(0, FencePointsToSpawn.Count);
                var npcSpawned = FenceNPCToSpawn[i].gameObject;
                npcSpawned.gameObject.SetActive(true);                    
                
                npcSpawned.LeanMove(FencePointsToSpawn[sortIndex].position, 0f);
                npcSpawned.LeanRotateY(FencePointsToSpawn[sortIndex].rotation.eulerAngles.y, 0f);

                FencePointsToSpawn.Remove(FencePointsToSpawn[sortIndex]); // Evita de Spawnar no mesmo Lugar
            } else {
                Debug.LogWarning("Atenção - há mais NPC's 'FenceNpcs' para spawn do que SpawnPoints. Cancelando Spawn do NPC: " + FenceNPCToSpawn[i].gameObject.name);
                break;
            }
        }
    }
}