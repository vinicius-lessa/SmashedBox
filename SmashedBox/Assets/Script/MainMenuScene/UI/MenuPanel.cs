using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour
{
    public GameObject tutorialButton;
    public static bool tutorialIsShowed;

    // # MenuPanel Button Selection System
    public GameObject menuFirstButton;
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;
    private bool isFirstSelected = false; // Not play "selection" for the firs selected
    EventSystem m_EventSystem;

    private void Start() {
        tutorialIsShowed = PlayerPrefs.GetInt("TutorialIsShowed", 0) == 0 ? false : true;

        if (tutorialIsShowed)
            tutorialButton.SetActive(true); // default is inactive
    }

    private void OnEnable() {
        m_EventSystem = EventSystem.current;

        //clear selected object
        m_EventSystem.SetSelectedGameObject(null);
        //set a new selected object
        m_EventSystem.SetSelectedGameObject(menuFirstButton);        
    }

    private void Update() {
        // Feito para ativar a "seta" do item selecionado na UI 
        if (m_EventSystem.currentSelectedGameObject != null) // Caso clique em qualquer outro lugar da tela
        {
            GetLastGameObjectSelected();
            if (!m_EventSystem.currentSelectedGameObject.CompareTag("UIDontUseArrowSelection"))
                m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(true);

            if (lastSelectedGameObject != null)
            {
                if (!lastSelectedGameObject.CompareTag("UIDontUseArrowSelection"))
                    lastSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(false);
            }
        } else {
            m_EventSystem.SetSelectedGameObject(menuFirstButton); // Se clicar na tela, volta para o primeiro botão
        }
    }

    private void GetLastGameObjectSelected() {        
        if (m_EventSystem.currentSelectedGameObject != currentSelectedGameObject_Recent) {            
            if (isFirstSelected)
            {
                FindObjectOfType<AudioManager>().Play("[FX] SwitchButtonSelection");
            }
            isFirstSelected = true;

            lastSelectedGameObject = currentSelectedGameObject_Recent;

            currentSelectedGameObject_Recent = m_EventSystem.currentSelectedGameObject;
        }
    }
        
}