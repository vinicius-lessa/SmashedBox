using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public GameObject GamepadInstructions;
    public GameObject KeyBoardInstructions;

    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(CheckInput());

        // if (Input.GetJoystickNames().Length == 0) {
        //     Debug.Log("Keyboard");            
        //     KeyBoardInstructions.SetActive(true);
        // } else {
        //     Debug.Log("Controle");
        //     Debug.Log(Input.GetJoystickNames());
        //     GamepadInstructions.SetActive(true);
        // }
    }

    private void Update() {       
    
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

    IEnumerator CheckInput() {
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