using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject optionsFirstButton;
    public Slider volumeSlider;
    public Toggle muteMusicToogle;

    // Selected Button
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;
    EventSystem m_EventSystem;

    private bool isFirstSelected = false;

    void OnEnable()
    {
        m_EventSystem = EventSystem.current;
        //clear selected object
        m_EventSystem.SetSelectedGameObject(null);
        //set a new selected object
        m_EventSystem.SetSelectedGameObject(optionsFirstButton);

        volumeSlider.value = PlayerPrefs.GetFloat("VolumeSlider", 1); // Default is 1

        int binaryConversion = PlayerPrefs.GetInt("MuteMusicToogle", 0); // Default is False (Music is not Muted)

        if (binaryConversion == 1) // true
        {
            muteMusicToogle.isOn = true;
        } else { // false
            muteMusicToogle.isOn = false;
        }     
    }

    private void Update() {
        // Feito para ativar a "seta" do item selecionado na UI 
        if (m_EventSystem.currentSelectedGameObject != null)
        {
            GetLastGameObjectSelected();
            if (m_EventSystem.currentSelectedGameObject.gameObject.GetComponent<Button>() != null) // Se for Button (Back)
            {
                Color specificblue;
                ColorUtility.TryParseHtmlString("#3B7CFF", out specificblue);
                m_EventSystem.currentSelectedGameObject.transform.Find("[Text] Back").gameObject.GetComponent<TextMeshProUGUI>().color = specificblue;
                m_EventSystem.currentSelectedGameObject.transform.Find("[Text] Arrow").gameObject.GetComponent<TextMeshProUGUI>().color = specificblue;
            } else {
                // Ativa a "Seta"
                m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(true);
            }            

            if (lastSelectedGameObject != null)
            {
                if (lastSelectedGameObject.gameObject.GetComponent<Button>() != null) // Se for Button (Back)
                {                    
                    lastSelectedGameObject.transform.Find("[Text] Back").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                    lastSelectedGameObject.transform.Find("[Text] Arrow").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                } else {
                    lastSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(false);
                }
            }
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

    public void ChangeVolume()
    {        
        PlayerPrefs.SetFloat("VolumeSlider", volumeSlider.value);
        float finalVolume = PlayerPrefs.GetFloat("VolumeSlider", 1);
        AudioListener.volume = finalVolume;
        // // Debug.Log("O volume é: " + finalVolume);
    }

    public void MuteGeneralVolume()
    {
        if (muteMusicToogle.isOn) {
            PlayerPrefs.SetInt("MuteMusicToogle", 1);            
            if (FindObjectOfType<AudioManager>().IsPlaying("[TK] MainMenuTheme")) {
                FindObjectOfType<AudioManager>().Mute("[TK] MainMenuTheme"); // Mute actual music
            }
        } else {
            PlayerPrefs.SetInt("MuteMusicToogle", 0);
            if (FindObjectOfType<AudioManager>().IsPlaying("[TK] MainMenuTheme")) {
                FindObjectOfType<AudioManager>().Unmute("[TK] MainMenuTheme"); // Unmute actual music
            } else {
                FindObjectOfType<AudioManager>().Play("[TK] MainMenuTheme"); // play default music
            }
        }
        
        float finalVolume = PlayerPrefs.GetFloat("VolumeSlider", 1);
        AudioListener.volume = finalVolume;
        // // Debug.Log("O volume é: " + finalVolume);
    }    

    public void BackButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        transform.gameObject.SetActive(false);
        menuPanel.SetActive(true);
    }
}