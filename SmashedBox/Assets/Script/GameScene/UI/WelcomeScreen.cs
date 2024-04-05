/*
* @Documentaion
* 
* DESCRIPTION
*       Este scrit está atrelado ao GameObject do tipo UI presente na hieranquia da SCENE "GAME"
*       Apresenta tela inicial de boas vindas toda vez que o jogo for aberto (caso playerID não esteja preenchido).
*       Captura e trata os dados recebidos via input informados pelo player para o 'Scoreboard'
*
* DATES
*      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
*      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
* 
* NOTES & REFERENCES
*       LeanTween: https://assetstore.unity.com/packages/tools/animation/leantween-3595
*       
*       Formatos de Movimento (Boost - setEase...): https://codepen.io/jhnsnc/pen/LpVXGM
*
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class WelcomeScreen : MonoBehaviour
{    
    public TMP_InputField playerNameField;
    public Button submitButton;
    public GameObject FirstButton;
    public GameObject Player;
    public GameObject GameManager;
    public GameObject UIGameplayText;
    public GameObject PlayerHud;

    // Selected Button
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;
    private bool isFirstSelected = false;

    EventSystem m_EventSystem;

    private void Start() { 
        if (playerNameField.text.Length == 0)
            submitButton.interactable = false;

        m_EventSystem = EventSystem.current;

        // CrossFadeIn
        FindObjectOfType<CrossFade>().CrossFadeIn(1f);

        AudioListener.volume = PlayerPrefs.GetFloat("VolumeSlider", 1); // General Volume
        
        playerNameField.text = PlayerPrefs.GetString("PlayerName", ""); // Fills automatically with PlayerName saved previously
        
        
        EventSystem.current.SetSelectedGameObject(null);        // Clear selected object
        EventSystem.current.SetSelectedGameObject(FirstButton); // Set a new selected object
                                     
        // Verifies MuteMusic Toogle in OptionsMenu.cs
        if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0) // Default is False - Not Muted        
            FindObjectOfType<AudioManager>().FadeAudio("[TK] MainMenuTheme", 2f, true); // Fade In
    }

    private void Update() {
        // Feito para ativar a "seta" do item selecionado na UI 
        if (m_EventSystem.currentSelectedGameObject != null)
        {
            GetLastGameObjectSelected();
            if (m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow") != null)
            {
                // Ativa a "Seta"
                m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(true);

                // Se for Button
                if (m_EventSystem.currentSelectedGameObject.gameObject.GetComponent<Button>() != null)
                {
                    Color darkGreen;
                    ColorUtility.TryParseHtmlString("#386727", out darkGreen);

                    m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] Play").gameObject.GetComponent<TextMeshProUGUI>().color = darkGreen;
                }
            }         

            if (lastSelectedGameObject != null)
            {
                lastSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(false);
                // Se for Button
                if (lastSelectedGameObject.gameObject.GetComponent<Button>() != null)
                {
                    // Debug.Log("É botão");
                    lastSelectedGameObject.transform.Find("[TMPro] Play").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }
        }

        // VerifyInput();
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

    // Chamado ao clicar em "PLAY !"
    public void EnterPlay() {     
        if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0) // Default is False - Not Muted        
            FindObjectOfType<AudioManager>().FadeAudio("[TK] MainMenuTheme", 2f, false); // Fade Out

        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        
        string name = playerNameField.text;
        
        // Sets Player Name
        if (PlayerPrefs.GetString("PlayerName", "") != name.ToString().ToUpper()) {
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.SetString("PlayerName", name.ToString().ToUpper());
        }         

        StartCoroutine(CallGamePlay());        
    }

    public void VerifyInput(){ 
        // Somente ativa o Botão de "Play" se atender a condição
        submitButton.interactable = (playerNameField.text.Length >= 3);
    }
    
    public void InputToUpper(){
        playerNameField.text = playerNameField.text.ToUpper(); // Transforma em UPPERCASE
    }

    public IEnumerator CallGamePlay()
    {
        yield return new WaitForSeconds(1.6f);

        // Desativa tela Inicial
        transform.gameObject.SetActive(false);
        
        // STARTS THE GAME
        Player.gameObject.SetActive(true);
        GameManager.gameObject.SetActive(true);
        UIGameplayText.SetActive(true);
        PlayerHud.gameObject.SetActive(true);
    }
}