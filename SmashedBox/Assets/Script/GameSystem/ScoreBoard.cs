// Video Aula: https://youtu.be/4W90-mh70JY

/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      ...
 *
 * DATES
 *      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
 *      
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreBoard : MonoBehaviour
{
    /*
    public void CallGetHighScore(string typeSeek){                
        StartCoroutine(GetHighScore(typeSeek));
    } 
    */

    /*
    // ### HTTP - MÉTODO GET - CONSULTA HIGHSCORES   
    public IEnumerable<JSONObject> GetHighScore(string searchType)
    {
        if (searchType == "")
            // Debug.LogWarning("Nenhum parâmetro de busca foi passado! Verifique.");
            yield break;        
        
        // [LOCAL]
        string uriLocal = "http://localhost/sqlconnect/highscore.php/" + typeSeek;
        // [WEB]
        string uriWeb = "http://smashed-box-backend.herokuapp.com/sqlconnect/highscore.php/" + typeSeek;

        using (UnityWebRequest www = UnityWebRequest.Get(uriWeb))
        {

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.Log(www.responseCode.ToString() + " - " + www.error);
            } 
            else 
            {
                string receivedText = www.downloadHandler.text;
                
                // JSON recebido
                JSONObject jsonValue = new JSONObject(receivedText);

                if (typeSeek == "allData") {
                    HighScoreTable.receiveDataAll(jsonValue);
                } else {
                    GameManager.ParsePlayerScore(jsonValue);
                }

                // // Debug.Log(jsonValue.Count); // [Row Numbers from the Database table]
                // // Debug.Log(jsonValue.ToString());
                // // Debug.Log(jsonValue.type); // [Should to be ARRAY]

                //Testando consulta
                // for(int i = 0; i < jsonValue.list.Count; i++) {
                //     // // Debug.Log(i.ToString());
                //     // Debug.Log("Posição " + i + ": " + jsonValue[i].GetField("id").ToString() + jsonValue[i].GetField("playername").ToString() + " - " + jsonValue[i].GetField("score").ToString());
                // }

                yield break;
            }
        }
        yield break;
        
    }
    */

    // # INCLUI/ALTERA HIGHSCORES
    public void UpdateHighScore(int id, int score, string playerName)
    {
        PlayerPrefs.SetInt("PlayerPersonalBest", score);
    }          
}