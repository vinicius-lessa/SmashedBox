using UnityEngine;

/*
    Formatos de Movimento (setEase...):
        https://codepen.io/jhnsnc/pen/LpVXGM
        http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
*/

public class WelcomeUp : MonoBehaviour
{
    public void WelcomeToUp () {
        transform.LeanMoveLocal(new Vector2(0, 500), 1.5f).setEaseInQuart();
    }
}
