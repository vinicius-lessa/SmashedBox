using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLoader : MonoBehaviour
{        
    private float transitonTime = 1f;
    public GameObject loadingPanel;
    public GameObject menuPanel;

    public void LoadNextLevel(bool tutorialShowed) {
        if (tutorialShowed)
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 2)); // calls game scene            
        else
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1)); // calls tutorial scene
    }

    // CoRoutine
    IEnumerator LoadLevel(int levelIndex) 
    {
        FindObjectOfType<AudioManager>().FadeAudio("[TK] MainMenuTheme", transitonTime, false); // Fade Out            

        if (levelIndex == SceneManager.GetActiveScene().buildIndex + 2) { // GameScene            
            FindObjectOfType<CrossFade>().CrossFadeOut(transitonTime, .1f); // With Delay
            yield return new WaitForSeconds(transitonTime);            

            StartCoroutine(LoadAsynchronously(levelIndex)); // Loading
        } else {
            FindObjectOfType<CrossFade>().CrossFadeOut(transitonTime, .1f); // With Delay
            SceneManager.LoadScene(levelIndex); // Tutorial
        }

        menuPanel.SetActive(false);
    }

    IEnumerator LoadAsynchronously(int levelIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        loadingPanel.SetActive(true);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
                        
            loadingPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Loading " + Mathf.Round(progress * 100f) + "%");
            // Debug.Log(Mathf.Round(progress * 100f));            
            
            yield return null;
        }                       
    }
}