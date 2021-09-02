using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    Formatos de Movimento (setEase...):
        https://codepen.io/jhnsnc/pen/LpVXGM
*/

public class BoostBar : MonoBehaviour
{
    Slider sliderBoost;
    bool boostIsFully = false;

    private void Start() {
        sliderBoost = GameObject.Find("[Slider] Boost").GetComponent<Slider>();
    }

    private void Update() {
        if(sliderBoost.value >= 4.6f){
            if(!boostIsFully){
                boostIsFully = true;
                StartCoroutine(BoostBarFull());                
            }
        } else {
            boostIsFully = false;
        }
    }

    private IEnumerator BoostBarFull() {
        if (!boostIsFully)
            yield break;

        // int y = 1;
        while(boostIsFully)
        {
            transform.LeanScale(new Vector2(1.2f, 1.2f), .5f);
            yield return new WaitForSeconds(.6f);
            transform.LeanScale(new Vector2(1f, 1f), .5f);
            yield return new WaitForSeconds(.6f);
        }

        BoostBarFull();
    }
}