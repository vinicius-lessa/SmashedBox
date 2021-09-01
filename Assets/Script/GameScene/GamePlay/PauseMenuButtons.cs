using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuButtons : MonoBehaviour
{
    public GameObject pauseFirstButton;
    public void OnEnable() {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);        
    }
}
