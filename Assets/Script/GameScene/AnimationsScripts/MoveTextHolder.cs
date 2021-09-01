using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Description: 
        Este scrip é um componente do GameObject estânciado no script "GameManager.cs" e que usa como base o Prefab [TextHolder] HitStone2.
        Ele é resposável por:
            - Mover o Hit Text até a posição do SCORE do player na Tela
            - Destruir o objeto que se tornará invisível após alguns segundos, limpando assim a memória Ram.

    Packges:
        LEAN TWEEN:
            API Documentarion:
                http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html            
            
            Formatos de Movimento (setEase...):
                https://codepen.io/jhnsnc/pen/LpVXGM
        
*/

public class MoveTextHolder : MonoBehaviour
{
    private GameObject parentObj;    

    void Start()
    {
        StartCoroutine(MovelHitText()); // Animation
        
        parentObj = this.gameObject.transform.parent.gameObject;        
        Destroy(parentObj, 5f); 
    }

    IEnumerator MovelHitText() {        
        float secondsDefaultAnin  = 1.1f;
        float secondsMovingAnin   = .3f;
        
        yield return new WaitForSeconds(secondsDefaultAnin); // Wait Initial Animation

        parentObj.transform.LeanMoveLocal(new Vector3(-4.8f, 5.8f, -6.5f), secondsMovingAnin); // Moves TextHolder to Score Postion
        yield return new WaitForSeconds(secondsMovingAnin);        
    }
}