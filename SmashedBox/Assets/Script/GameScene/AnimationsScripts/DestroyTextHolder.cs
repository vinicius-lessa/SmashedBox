using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    DOCUMENTATION

    Description: 
        Este scrip é um componente do GameObject estânciado no script "GameManager.cs" e que usa como base o Prefab [TextHolder] HitStone1.
        Ele é resposável por destruir o objeto que se tornará invisível após alguns segundos, limpando assim a memória Ram.    
*/

public class DestroyTextHolder : MonoBehaviour
{
    void Start()
    {
        GameObject parent = this.gameObject.transform.parent.gameObject;        
        Destroy(parent, 2f); 
    }
}
