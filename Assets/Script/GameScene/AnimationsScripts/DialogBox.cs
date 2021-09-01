using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Formatos de Movimento (setEase...):
        https://codepen.io/jhnsnc/pen/LpVXGM
*/

public class DialogBox : MonoBehaviour
{
    public void DialogBoxDown () {
        transform.LeanMoveLocal(new Vector2(0, -650), 1.5f).setEaseInQuart();
    }
}
