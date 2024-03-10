using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Responsible for showing the GameOver screen when the player is destroyed
 *
 * DATES
 *      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
 *
 * NOTES & REFERENCES
 *      LeanTween: https://assetstore.unity.com/packages/tools/animation/leantween-3595
 *      Formatos de Movimento (Boost - setEase...): https://codepen.io/jhnsnc/pen/LpVXGM
 *
*/

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI feedBack;
    public GameObject newRecordText, gameOverFirstButton;
    private GameObject smashedText;

    private string textFeedBack;    
    // Para Gravação do SCORE em PLAYERPREFS
    public HighScoreTable HighScoreTable;

    // Selected Button
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent; 
    private Hashtable feedBackList = new Hashtable(); // FeedBack
    EventSystem m_EventSystem; // Cashing
    

    private bool isFirstSelected = false;    

    private void Start() {
        m_EventSystem = EventSystem.current;
    }

    private void OnEnable() {  
        smashedText = transform.Find("[Img] Smashed").gameObject;
        smashedText.transform.LeanSetLocalPosY(510f);
        smashedText.transform.LeanMoveLocalY(215f, 1f).setEaseInOutBack();
        

        feedBackList.Clear(); // Clear everytime enters the game
        //Debug.Log(feedBackList.Count);

        // Level1
        feedBackList.Add(0 , "A monkey would play Better than That!");
        feedBackList.Add(1 , "Are you Kidding? You should play PONG!");
        feedBackList.Add(2 , "This is Not for you! Surrender!");
        // Level2
        feedBackList.Add(10 , "Are you going to play that for real sometime?");
        feedBackList.Add(11 , "Congrats Chimpanzee!");
        feedBackList.Add(12 , "OK, I'll take it easy next time...");
        // Level3
        feedBackList.Add(20 , "You're getting better, aren't you?!");
        feedBackList.Add(21 , "Come on, you almost nailed it!");
        feedBackList.Add(22 , "Ok, that was close!");

        // Level4
        feedBackList.Add(30 , "YOU ARE THE BEST!!! (Just Kidding)");
        feedBackList.Add(31 , "I knew that you has born for that! CONGRATULATIONS!!!");
        feedBackList.Add(32 , "You surprised me square boy! SQUISHY !!!");

        //Debug.Log(feedBackList.Count);

        m_EventSystem = EventSystem.current;
        //clear selected object
        m_EventSystem.SetSelectedGameObject(null);
        //set a new selected object
        m_EventSystem.SetSelectedGameObject(gameOverFirstButton);
    }

    private void Update() {
        // Feito para ativar a "seta" do item selecionado na UI 
        if (m_EventSystem.currentSelectedGameObject != null) // Caso clique em qualquer outro lugar da tela
        {
            GetLastGameObjectSelected();        
            m_EventSystem.currentSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(true);

            if (lastSelectedGameObject != null)
            {
                lastSelectedGameObject.transform.Find("[TMPro] SelectedArrow").gameObject.SetActive(false);
            }
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

    public void Setup(int score, string name) {
        // Ativa GAME OVER SCREEN
        gameObject.SetActive(true);
            
        bool newPersonalRecord = false;
        int sortFeedBack;

        if (score > PlayerPrefs.GetInt("PlayerPersonalBest", 0)) 
        {
            int id = PlayerPrefs.GetInt("PlayerID", 0);
            newPersonalRecord = true;
            
            FindObjectOfType<HighScoreRegistration>().CallPostHighScore(id, score, name); // Valor é retornado em "GameManager.cs"
        }else {
            // Debug.Log("Score Menor ou Igual ao PERSONAL BEST.   #[GameOverScreen - ProcessGameOverScreen]");
        }        

        // Se for Novo RECORD
        if (newPersonalRecord) {            
            newRecordText.transform.gameObject.SetActive(true);
        } // Caso não, segue o Padrão
        else if (!newPersonalRecord) {
            if (score <= 20) {
                sortFeedBack = Random.Range(0, 3);
                textFeedBack = feedBackList[sortFeedBack].ToString();             
            } else if (score > 20 && score <= 40){
                sortFeedBack = Random.Range(10, 13);
                textFeedBack = feedBackList[sortFeedBack].ToString();
            } else if (score > 40 && score <= 60) {
                sortFeedBack = Random.Range(20, 23);
                textFeedBack = feedBackList[sortFeedBack].ToString();
            } else if (score > 60) {
                sortFeedBack = Random.Range(30, 33);
                textFeedBack = feedBackList[sortFeedBack].ToString();
            }

            feedBack.transform.gameObject.SetActive(true);
            feedBack.text = textFeedBack;
        }        

        pointsText.text = PlayerPrefs.GetString("PlayerName") + ", you survived for " + score.ToString() + " SECONDS";        
    }

    public void RestartButton () {
        // Debug.Log("Clicou em Restart.");
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        GameManager.score = 0;
        // Call Game Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButton () {
        // Debug.Log("Clicou em Exit.");
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        // Limpa PlayerPrefs        
        
        // PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("PlayerPersonalBest");

        GameManager.score = 0;
        
        // Call Menu Scene
        SceneManager.LoadScene(0);
    }

    public void WorldRanking () {        
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        transform.gameObject.SetActive(false);
        // Call HighScore Table
        HighScoreTable.gameObject.SetActive(true);
    }
}