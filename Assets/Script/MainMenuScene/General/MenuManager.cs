using UnityEngine;
using System.Collections;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 19/07/2021

    Descrição:
        Este script é responsável pela apresentação do menu principal e seu 3 ou 4 botões, "PLAY" - "OPTIONS" - "QUIT" - "Tutorial".
        O último será apresentado ou não dependendo se o tutorial já foi exibido anteriormente.
        Além disto, aqui é definido o esqueme de seleção dos botões para navegação com GAMEPAD ou DIRECIONAIS no teclado, ativando e desativando a "seta" do lado do botão selecionado.
        Também define o botão selecionado por DEFAULT.

    Observações:
        Modo de calcular o progresso do carregamento de uma operação (no caso a troca da scene) - // float progress = Mathf.Clamp01(operation.progress / .9f);

*/

public class MenuManager : MonoBehaviour
{    
    // # Public GameObjects
    public GameObject menuPanel;
    public GameObject connectionTestPanel;    
    
    // # Called when certain Button is hited
    public GameObject optionsPanel;

    private void Start() {
        AudioListener.volume = PlayerPrefs.GetFloat("VolumeSlider", 1);

        // Verifies MuteMusic Toogle in OptionsMenu.cs
        if (!(PlayerPrefs.GetInt("MuteMusicToogle", 0) == 1)) // Default is False - Not Muted
            FindObjectOfType<AudioManager>().FadeAudio("[TK] MainMenuTheme", 2f, true); // Fade In
    }    

    private void OnEnable() {
        FindObjectOfType<CrossFade>().CrossFadeIn(1f);     

        if(PlayerPrefs.GetInt("InternetConnection", 3) == 1) { // Connection has already been validated            
            menuPanel.SetActive(true);
        } else {
            connectionTestPanel.SetActive(true);            
        }
    }

    // # BUTTONS METHEDOS

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

        // Limpa PlayerName
        // PlayerPrefs.DeleteKey("PlayerName"); // Será carregado opcionalmente no Input do "InitialWelcome"
        
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("PlayerPersonalBest");
        PlayerPrefs.DeleteKey("InternetConnection");
        
        // PlayerPrefs.DeleteAll();
        
        Debug.Log("Quiting the game and Cleaning Data!");
        Application.Quit();
    }

    public void TutorialButton () {
        FindObjectOfType<LevelLoader>().LoadNextLevel(false); // Show tutorial
    }
}