using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

/*
### - DOC    

    Creator(s): VINÍCIUS LESSA (LessLax Studios)

    Date: 10/08/2021

    Description:
        Script utilizado somente em scene "Tutorial".
        Tutorial manager, atualmente atribuído como componente ao Painel que contém o botão de "SKIP"

    Changes:
        11/08/2021 - Vinícius Lessa:
        Mudança do GameObject o qual o script estava atribuído de "Painel" para "TutorialManager".
        Realizada as devidas alterações necessárias para referenciar objetos utilizados, além disto, adicionado sistema de seleção de botões e ativação
        da seta lateral dos mesmos.
        Também é tratado o processo de passar os vídeos de tutoriasi que no total são 4 deles, através do "click" do botão "continue".

        16/08/2021 - Vinícius Lessa:
        Mudança referente ao processo de carregamento da cena "Game" (quando a mesma for chamada), a fim de dar um retorno ao player sobre o progresso do carregamento do jogo.

*/

public class TutorialManager : MonoBehaviour
{    
    private float transitonTime = 1f; // AudioOut and FadeOut  
    private int videoSelected = 0;  // Saves the Current Playing Video
    private bool isFirtVideo;
    
    // # External GameObjects
    public TextMeshProUGUI videoCounter;
    public List<GameObject> TutorialVideos = new List<GameObject>();
    public GameObject backButtonObj;
    public GameObject skipButtonObj;
    public Button continueButton;

    // # Scene Button Selection
    public GameObject tutorialFirstButton; // First Selected Button
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;
    private bool isFirstSelected = false; // NOT play the change button SFX at first time
    EventSystem m_EventSystem; // Cashing

    // # Tutorial
    private bool tutorialIsShowed;

    // # Game Scene
    public GameObject videoPanel;
    public GameObject loadingScreen;
 
    private void Start() {
        PlayerPrefs.SetInt("TutorialIsShowed", 1); // Tutorial showed
        
        isFirtVideo = true;
        NextVideoButton();

        // Verifies MuteMusic Toogle in OptionsMenu.cs
        if (!(PlayerPrefs.GetInt("MuteMusicToogle", 0) == 1)) // Default is False - Not Muted
            FindObjectOfType<AudioManager>().FadeAudio("[TK] Monkey Warhol", 2f, true); // Fade In

        return;
    }

    private void OnEnable() {
        FindObjectOfType<CrossFade>().CrossFadeIn(2f);

        tutorialIsShowed = PlayerPrefs.GetInt("TutorialIsShowed", 0) == 0 ? false : true;
        
        if (tutorialIsShowed) {
            backButtonObj.SetActive(true);
        } else {
            skipButtonObj.SetActive(true);
        }
        

        m_EventSystem = EventSystem.current;

        //clear selected object
        m_EventSystem.SetSelectedGameObject(null);
        //set a new selected object
        m_EventSystem.SetSelectedGameObject(tutorialFirstButton);        
    }

    private void Update() {
        // Show/Hide the arrow of each button on UI
        if (m_EventSystem.currentSelectedGameObject != null)
        {
            GetLastGameObjectSelected();
            if (!m_EventSystem.currentSelectedGameObject.CompareTag("UIDontUseArrowSelection")) {
                m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(true);
            } else { // BackButton
                Color specificblue;
                ColorUtility.TryParseHtmlString("#3B7CFF", out specificblue);
                m_EventSystem.currentSelectedGameObject.transform.Find("[Text] Back").gameObject.GetComponent<TextMeshProUGUI>().color = specificblue;
                m_EventSystem.currentSelectedGameObject.transform.Find("[Text] Arrow").gameObject.GetComponent<TextMeshProUGUI>().color = specificblue;
            }            

            if (lastSelectedGameObject != null) {
                if (!lastSelectedGameObject.CompareTag("UIDontUseArrowSelection")) {
                    lastSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(false);
                } else {                    
                    lastSelectedGameObject.transform.Find("[Text] Back").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                    lastSelectedGameObject.transform.Find("[Text] Arrow").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }
        } else {
            m_EventSystem.SetSelectedGameObject(tutorialFirstButton); // Se clicar na tela, volta para o primeiro botão
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

    public void NextVideoButton()
    {
        // Changes the Video on Display        
        if (!isFirtVideo) {
            FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
            TutorialVideos[videoSelected].transform.GetChild(0).GetComponent<VideoPlayer>().Stop();            

            int previousVideo = videoSelected;
            videoSelected = videoSelected + 1; // Update selected Video

            TutorialVideos[videoSelected].gameObject.SetActive(true);
            TutorialVideos[previousVideo].gameObject.SetActive(false);
            TutorialVideos[videoSelected].transform.GetChild(0).GetComponent<VideoPlayer>().Play();                
        } else { // FirstVideo
            TutorialVideos[videoSelected].gameObject.SetActive(true);
            TutorialVideos[videoSelected].transform.GetChild(0).GetComponent<VideoPlayer>().Play();
            isFirtVideo = false;
        }
        
        videoCounter.SetText((videoSelected+1) + "/" + (TutorialVideos.Count)); // 1 / ...

        // Defines if the "Continue" button is Enable/Disable
        continueButton.interactable = (!(videoSelected == (TutorialVideos.Count - 1)));
        
        if (!continueButton.interactable) {
            Color disableGray;
            ColorUtility.TryParseHtmlString("#C8C3C3", out disableGray);
            
            continueButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = disableGray;

            if (tutorialIsShowed) {
                var remainButtonObj = GameObject.Find("[Button] BackToMenu");
                m_EventSystem.SetSelectedGameObject(remainButtonObj);

                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.None;
                remainButtonObj.GetComponent<Button>().navigation = customNav;                
            } else {
                var remainButtonObj = GameObject.Find("[Button] SkipScene");
                m_EventSystem.SetSelectedGameObject(remainButtonObj);

                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.None;
                remainButtonObj.GetComponent<Button>().navigation = customNav;
            }            
        }
    }

    public void SkipButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        FindObjectOfType<AudioManager>().FadeAudio("[TK] Monkey Warhol", transitonTime, false); // Fade Out

        TutorialVideos[videoSelected].transform.GetChild(0).GetComponent<VideoPlayer>().Stop();
        // TutorialVideos[videoSelected].gameObject.SetActive(false);

        LoadNextLevel();     
    }

    public void BackButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        FindObjectOfType<AudioManager>().FadeAudio("[TK] Monkey Warhol", transitonTime, false); // Fade Out

        TutorialVideos[videoSelected].transform.GetChild(0).GetComponent<VideoPlayer>().Stop();

        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1)); // Back to Menu
    }    
    
    private void LoadNextLevel() {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    
    IEnumerator LoadLevel(int levelIndex)
    {        
        FindObjectOfType<CrossFade>().CrossFadeOut(transitonTime, 0.1f); // CrossFadeOut (transition, delay)
        yield return new WaitForSeconds(transitonTime);
        
        if (levelIndex == SceneManager.GetActiveScene().buildIndex + 1) { // GameScene            
            StartCoroutine(LoadAsynchronously(levelIndex));
        } else {
            SceneManager.LoadScene(levelIndex);            
        }
    }

    IEnumerator LoadAsynchronously(int levelIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        videoPanel.SetActive(false);
        loadingScreen.SetActive(true);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            loadingScreen.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Loading " + Mathf.Round(progress * 100f) + "%");
            // Debug.Log(Mathf.Round(progress * 100f));
            //Debug.Log(progress);
            
            yield return null;
        }
    }
}