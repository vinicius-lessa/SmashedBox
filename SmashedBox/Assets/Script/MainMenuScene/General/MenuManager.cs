using UnityEngine;
using System.Collections;

/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Este script é responsável pela apresentação do menu principal e seus 3 ou 4 botões, "PLAY" - "OPTIONS" - "QUIT" - "Tutorial (ocasional)".
 *      O último será apresentado dependendo do tutorial já ter sido exibido anteriormente.
 *      Aqui é definido o esquema de seleção dos botões para navegação com GAMEPAD ou DIRECIONAIS no teclado, ativando e desativando a "seta" do lado do botão selecionado.
 *      Também define o botão selecionado por DEFAULT.
 *
 * DATES
 *      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
 *      
 * NOTES
 *      Modo de calcular o progresso do carregamento de uma operação (no caso a troca da scene)
 *      // float progress = Mathf.Clamp01(operation.progress / .9f);
 *   
*/

public class MenuManager : MonoBehaviour
{    
    // Public GameObjects
    public GameObject menuPanel;
    // public GameObject connectionTestPanel;    
    
    // Called when certain Button is clicked
    public GameObject optionsPanel;

    private void Start() {
        AudioListener.volume = PlayerPrefs.GetFloat("VolumeSlider", 1);

        // Verifies MuteMusic Toogle in OptionsMenu.cs
        if (!(PlayerPrefs.GetInt("MuteMusicToogle", 0) == 1)) // Default is False - Not Muted
            FindObjectOfType<AudioManager>().FadeAudio("[TK] MainMenuTheme", 2f, true); // Fade In
    }    

    private void OnEnable() {
        FindObjectOfType<CrossFade>().CrossFadeIn(1f);     

        //if(PlayerPrefs.GetInt("InternetConnection", 3) == 1) // Connection has already been validated
            menuPanel.SetActive(true);
        // else
            // connectionTestPanel.SetActive(true);
    }

    // BUTTONS METHEDOS
    public void PlayGameButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");        

        // menuPanel.SetActive(false);
        FindObjectOfType<LevelLoader>().LoadNextLevel(MenuPanel.tutorialIsShowed);
    }
    
    public void OptionsButton()
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");

        // Load Options Screen
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void QuitGameButton() 
    {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        
        // PlayerPrefs.DeleteKey("PlayerID");
        // PlayerPrefs.DeleteKey("PlayerPersonalBest");
        // PlayerPrefs.DeleteKey("InternetConnection");
        
        // PlayerPrefs.DeleteAll();
        
        Debug.Log("Quiting the game and Cleaning Data!");
        Application.Quit();
    }

    public void TutorialButton () {
        FindObjectOfType<LevelLoader>().LoadNextLevel(false); // Show tutorial
    }
}