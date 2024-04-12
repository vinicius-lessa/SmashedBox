/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Script utilizado somente em scene "Tutorial".
 *      Tutorial manager, atualmente atribuído como componente ao Painel que contém o botão de "SKIP"
 *
 * DATES
 *      10/08/2021 - Creation of script
 *      11/08/2021
 *          Mudança do GameObject o qual o script estava atribuído de "Painel" para "TutorialManager". Realizada as devidas alterações necessárias para referenciar objetos utilizados, além disto, adicionado sistema de seleção de botões e ativação
 *          da seta lateral dos mesmos. Também é tratado o processo de passar os vídeos de tutoriasi que no total são 4 deles, através do "click" do botão "continue".
 *      16/08/2021
 *          Mudança referente ao processo de carregamento da cena "Game" (quando a mesma for chamada), a fim de dar um retorno ao player sobre o progresso do carregamento do jogo.
 *      11/04/2024
 *          Correction for the video not playing in WebGL build game - Now the videos will be played by URL (path)
 *   
 * METHODS
 *      ...
 *   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{    
    private float transitonTime = 1f; // AudioOut and FadeOut  
    private int videoIndex = 0;  // Saves the Current Playing Video
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

    //[SerializeField] string videoFileName;
    

    void Start() {
        PlayerPrefs.SetInt("TutorialIsShowed", 1); // Tutorial IS Showed now
        
        isFirtVideo = true;
        NextVideoButton();

        // Verifies MuteMusic Toogle in OptionsMenu.cs
        if (!(PlayerPrefs.GetInt("MuteMusicToogle", 0) == 1)) // Default is False - Not Muted
            FindObjectOfType<AudioManager>().FadeAudio("[TK] Monkey Warhol", 2f, true); // Fade In

        return;
    }

    void OnEnable() {
        FindObjectOfType<CrossFade>().CrossFadeIn(2f);

        tutorialIsShowed = PlayerPrefs.GetInt("TutorialIsShowed", 0) == 0 ? false : true;
        
        if (tutorialIsShowed)
            backButtonObj.SetActive(true);
        else
            skipButtonObj.SetActive(true);        
        
        m_EventSystem = EventSystem.current;
        
        m_EventSystem.SetSelectedGameObject(null); //clear selected object        
        m_EventSystem.SetSelectedGameObject(tutorialFirstButton); //set a new selected object
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
                FindObjectOfType<AudioManager>().Play("[FX] SwitchButtonSelection");

            isFirstSelected = true;
            lastSelectedGameObject = currentSelectedGameObject_Recent;
            currentSelectedGameObject_Recent = m_EventSystem.currentSelectedGameObject;
        }
    }

    public void NextVideoButton()
    {
        GameObject videoParent  = TutorialVideos[videoIndex];
        VideoPlayer videoPlayer = videoParent.transform.GetChild(0).GetComponent<VideoPlayer>();

        // First Video Name and Path
        string videoFileName, videoPath;   // File Name (is the name of the Video Player GameObject)

        if (videoPlayer)
        {
            if (!isFirtVideo)
            {
                FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm"); // Audio FX

                videoPlayer.Stop(); // Stop Previous Video

                int previousVideo = videoIndex;
                TutorialVideos[previousVideo].gameObject.SetActive(false);

                // Next Video
                videoIndex++;
                videoParent = TutorialVideos[videoIndex];
                videoParent.gameObject.SetActive(true);

                // First Video Name and Path
                videoFileName = videoParent.transform.GetChild(0).name + ".mp4"; // File Name is the GameObject Name
                videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

                videoPlayer = videoParent.transform.GetChild(0).GetComponent<VideoPlayer>();
                videoPlayer.url = videoPath;
                videoPlayer.Play();
            }
            else // FirstVideo
            {
                isFirtVideo = false;

                // First Video Name and Path
                videoFileName = videoParent.transform.GetChild(0).name + ".mp4"; // File Name is the GameObject Name
                videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

                videoParent.SetActive(true);
                videoPlayer = videoParent.transform.GetChild(0).GetComponent<VideoPlayer>();
                videoPlayer.url = videoPath;
                videoPlayer.Play();            
            }
        }

        videoCounter.SetText((videoIndex+1) + "/" + (TutorialVideos.Count));

        // Defines if the "Continue" button is Enable/Disable
        continueButton.interactable = (!(videoIndex == (TutorialVideos.Count - 1)));
        
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

        TutorialVideos[videoIndex].transform.GetChild(0).GetComponent<VideoPlayer>().Stop();

        LoadNextLevel();     
    }

    public void BackButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        FindObjectOfType<AudioManager>().FadeAudio("[TK] Monkey Warhol", transitonTime, false); // Fade Out

        TutorialVideos[videoIndex].transform.GetChild(0).GetComponent<VideoPlayer>().Stop();

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