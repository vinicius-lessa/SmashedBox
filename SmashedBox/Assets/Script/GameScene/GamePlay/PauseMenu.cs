using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 08/07/2021

    Descrição:
        Esse SCRIPT é responsável por abrir e gerenciar a tela "PAUSE" durante o jogo
        
    Definições:
        var 'GameIsPaused' Public: Porque queremos acessas essa variável a partir de outros scripts
        var 'GameIsPaused' Static: Porque não queremos referênciar esse Scrip especificamente, mas sim verificar de forma rápida e fácil o valor desta variável
        Método "VOID": não é esperado nenhum retorno
        Input.GetKeyDown(KeyCode.Escape): quando a telca "esc" for pressionada
        var public GameObject pauseMenuUI: instancio o GameObject a qual deve ser inserido na interface da UNITY, podendo desta forma manipulá-lo
*/

public class PauseMenu : MonoBehaviour
{    
    PlayerControls controls;
    public WelcomeScreen InitialWelcome;
    public static bool GameIsPause = false;
    public GameObject pauseMenuUI;
    public GameManager GameManager;
    
    // Selected Button
    private GameObject currentSelectedGameObject_Recent; 
    EventSystem m_EventSystem;
    
    private bool isFirstSelected = false;
    void Awake() {
        m_EventSystem = EventSystem.current;
        controls = new PlayerControls();
        controls.Gameplay.Pause.performed += ctx => PauseJoystick();
    }

    void OnEnable() {
        controls.Gameplay.Enable();
    }

    void OnDisable()        
    {
        controls.Gameplay.Disable();
    }

    void PauseJoystick() {
        if (!GameManager.gameOverScreenShowed && !InitialWelcome.gameObject.activeSelf) {
            if( !GameIsPause ){
                PauseGame();
            } else {
                Resume();
            }
        }
    }

    void Update() {
        if (m_EventSystem.currentSelectedGameObject != null && pauseMenuUI.gameObject.activeSelf) // Caso clique em qualquer outro lugar da tela
            GetLastGameObjectSelected();        

        // KEYBOARD 
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.gameOverScreenShowed && !InitialWelcome.gameObject.activeSelf){
            if (GameIsPause) {
                Resume();
            } else {
                PauseGame();
            }
        }
    }

    private void GetLastGameObjectSelected() {
        if (m_EventSystem.currentSelectedGameObject != currentSelectedGameObject_Recent) {
            if (isFirstSelected)
                FindObjectOfType<AudioManager>().Play("[FX] SwitchButtonSelection");

            isFirstSelected = true;
            currentSelectedGameObject_Recent = m_EventSystem.currentSelectedGameObject;
        }
    }    

    // Returns to the Game
    public void Resume() 
    {
        FindObjectOfType<AudioManager>().Play("[FX] PauseAtivationSound");         
        // FindObjectOfType<AudioManager>().Unmute("WalkingGrass");
        
        if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0){ // SadViolin & Music is On
            if(GameManager.score < 40)
                FindObjectOfType<AudioManager>().Unmute(GameManager.initialThemeOnGame);
            else if (GameManager.score >= 40)
                FindObjectOfType<AudioManager>().Unmute("[TK] AreYouReadyDubStep");            
        }
        
        // Desativa Panel de Pause
        pauseMenuUI.SetActive(false);
        
        // Retoma tempo do Jogo
        Time.timeScale = 1f;
        
        // Seta variável como False
        GameIsPause = false;      
    }

    // Pause the TimeScale and Show PANEL
    void PauseGame() 
    {        
        if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0){ // SadViolin & Music is On
            if(GameManager.score < 40)
                FindObjectOfType<AudioManager>().Mute(GameManager.initialThemeOnGame);
            else if (GameManager.score >= 40)
                FindObjectOfType<AudioManager>().Mute("[TK] AreYouReadyDubStep");            
        }        
        FindObjectOfType<AudioManager>().Play("[FX] PauseAtivationSound");
        
        // Activates Pause Panel
        pauseMenuUI.SetActive(true);
        
        Time.timeScale = 0f;
        
        GameIsPause = true;       
    }

    // Call Menu Scene
    public void LoadMenu()
    {        
        FindObjectOfType<AudioManager>().Play("[FX] PauseAtivationSound");
        
        GameIsPause = false;

        // Clear Player Prefs
        PlayerPrefs.DeleteKey(GameManager.playerNameKey);
        PlayerPrefs.DeleteKey(GameManager.playerPersonalBestKey);

        Time.timeScale = 1f;
        GameManager.score = 0;        
        SceneManager.LoadScene(0);
    } 

    // Quit the Game
    public void Quit()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        FindObjectOfType<AudioManager>().Play("[FX] PauseAtivationSound");
        
        GameIsPause = false;

        // Clear Player Prefs
        PlayerPrefs.DeleteKey(GameManager.playerNameKey);
        PlayerPrefs.DeleteKey(GameManager.playerPersonalBestKey);
        // PlayerPrefs.DeleteAll();

        Application.Quit();
    } 
}
