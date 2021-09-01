using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    EXAMPLES OF USAGE:        
        // CrossFadeOut
        FindObjectOfType<CrossFade>().CrossFadeOut(1f, 0.3f); // With Delay
        FindObjectOfType<CrossFade>().CrossFadeOut(1f, 0f); // No Delay

        // CrossFadeIn
        FindObjectOfType<CrossFade>().CrossFadeIn(1f, 0.3f); // With Delay
        FindObjectOfType<CrossFade>().CrossFadeIn(1f, 0f); // No Delay        
    
    Formatos de Movimento (setEase...):
        https://codepen.io/jhnsnc/pen/LpVXGM
*/

public class CrossFade : MonoBehaviour
{
    private CanvasGroup background;    
    
    public void CrossFadeIn(float transitionTime)
    {
        background = gameObject.GetComponent<CanvasGroup>();
        background.alpha = 1;
        background.LeanAlpha(0,transitionTime);
    }

    public void CrossFadeOut(float transitionTime, float delay)
    {        
        background = gameObject.GetComponent<CanvasGroup>();
        background.alpha = 0;
        background.LeanAlpha(1, transitionTime);
            
        StartCoroutine(ClearBackground(transitionTime, delay));
    }

    IEnumerator ClearBackground(float transitionTime, float delay) {
        if (!(delay == 0))
            yield return new WaitForSeconds(transitionTime + delay);
        else       
            yield return new WaitForSeconds(transitionTime);

        background.alpha = 0;
    }
}
