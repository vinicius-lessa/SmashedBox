using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class InputFieldName : MonoBehaviour {
 
    public TMP_InputField _inputField;
    PlayerControls controls;
 
    void Awake() {
        controls = new PlayerControls();
        controls.UI.Cancelnput.performed += ctx => exitInputInteration();

        _inputField = GetComponent<TMP_InputField>();
    }
    
    void OnEnable() {
        controls.UI.Enable();
    }

    void OnDisable()        
    {
        controls.UI.Disable();
    }    


    void exitInputInteration ()
    {
        if (transform.gameObject.activeSelf) // Somente funciona na tela InitialWelcome
        {
            _inputField.DeactivateInputField();
        }
    }
}