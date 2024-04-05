/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      
 *
 * DATES
 *      2021 - Vinícius Lessa (LessLax): Creation of script 
 *
 * NOTES & REFERENCES 
 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public GameObject GamepadInstructions;
    public GameObject KeyBoardInstructions;

    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    void OnEnable()
    {
        StartCoroutine(CheckInput());       
    }

    private void Update()
    {
    
        if(Xbox_One_Controller == 1) {
            //do something
        }
        else if(PS4_Controller == 1) {
            KeyBoardInstructions.SetActive(false);
            GamepadInstructions.SetActive(true);
        }
        else{
            GamepadInstructions.SetActive(false);
            KeyBoardInstructions.SetActive(true);
        }            
    }

    IEnumerator CheckInput()
    {
        string[] names = Input.GetJoystickNames();
        
        for (int x = 0; x < names.Length; x++){
            // print(names[x].Length);
            if (names[x].Length == 19)
            {
                print("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                print("XBOX ONE CONTROLLER IS CONNECTED");
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;

            }
        }

        yield return new WaitForSeconds(1f);
        CheckInput();
    }
}