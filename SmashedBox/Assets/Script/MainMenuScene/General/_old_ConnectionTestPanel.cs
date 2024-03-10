// 03/10/2024 - This function was discontinued

//using System.Collections;
//using System.Collections.Generic;
/*using UnityEngine;
using UnityEngine.Networking;

public class ConnectionTestPanel : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject connectionErrorPanel;
    
    private void OnEnable()
    {
        StartCoroutine(CheckInternetConnection());        
    }

    private System.Collections.IEnumerator CheckInternetConnection(){
        int isTrue = 1;
        int isFalse = 0;
        // string url = "https://google.com";
        // string urlLocal = "http://localhost/sqlconnect/highscore.php/";
        string urlWeb = "http://smashed-box-backend.herokuapp.com/sqlconnect/highscore.php/";

        yield return new WaitForSeconds(2f);
        
        UnityWebRequest request = new UnityWebRequest(urlWeb);
        yield return request.SendWebRequest();        
        
        if (PlayerPrefs.GetInt("InternetConnection", 3) != 3)
            PlayerPrefs.DeleteKey("InternetConnection"); // delete if exists        

        // Sucesso na Requisição 
        if (request.error == null)
        {
            PlayerPrefs.SetInt("InternetConnection", isTrue);        
            // Debug.Log(PlayerPrefs.GetInt("InternetConnection").ToString() + " / there's internet connection - #InternetConnection.cs - IEnumerator CheckInternetConnection()");
            
            transform.gameObject.SetActive(false);
            menuPanel.SetActive(true);
        }
        // Erro na Requisição
        else
        {            
            PlayerPrefs.SetInt("InternetConnection", isFalse);
            // Debug.Log(PlayerPrefs.GetInt("InternetConnection").ToString() + " / there's no internet connection - #InternetConnection.cs - IEnumerator CheckInternetConnection()");

            transform.gameObject.SetActive(false);
            connectionErrorPanel.SetActive(true);
            yield break;
        }        
        
        yield return null;
    }
}*/
