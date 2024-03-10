using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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

public class HighScoreRegistration : MonoBehaviour
{
    public void CallGetHighScore(string typeSeek){                
        StartCoroutine(GetHighScore(typeSeek));
    }

    public void CallPostHighScore(int id, int score, string playerName) {
        bool validInputs = (
                (score > 0 && score < 99999)                        // Limitação do Score
            &&  (playerName.Length >= 3 && playerName.Length <= 8)  // Tamanho de PlayerName
        );

        if (validInputs)
        {
            StartCoroutine(PostHighScore(id, score, playerName));
        } 
        else 
        {
            // Debug.Log("Problemas na validação dos campos. Verifique!");
            return;
        } 
    }

    // public void CallPutHighScore(int id, int score, string playerName) {
    //     bool validInputs = (
    //             (id > 0)
    //         &&  (score > 0 && score < 99999)                        // Limitação do Score
    //         &&  (playerName.Length >= 3 && playerName.Length <= 8)  // Tamanho de PlayerName
    //     );

    //     if (validInputs)
    //     {
    //         StartCoroutine(PutHighScore(id, score, playerName));
    //     } 
    //     else 
    //     {
    //         // Debug.Log("Problemas na validação dos campos. Verifique!");
    //         return;
    //     } 
    // }

    // ### HTTP - MÉTODO GET - CONSULTA HIGHSCORES
    IEnumerator GetHighScore(string typeSeek)
    {    
        if (typeSeek == "") {
            // Debug.LogWarning("Nenhum parâmetro de busca foi passado! Verifique.");
            yield break;
        }       

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
                    GameManager.receiveDataGameManager(jsonValue);
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

    // ### HTTP - MÉTODO GET - INCLUI/ALTERA HIGHSCORES
    IEnumerator PostHighScore(int id, int score, string playerName)
    {     
        // [LOCAL]
        string uriLocal = "http://localhost/sqlconnect/highscore.php/" + playerName;
        // [WEB]
        string uriWeb = "http://smashed-box-backend.herokuapp.com/sqlconnect/highscore.php/" + playerName;
        
        // Form to be sended with the Request
        WWWForm form = new WWWForm();
        form.AddField("id"          , id                );        
        form.AddField("score"       , score             );
        form.AddField("playername"  , playerName        );

        // Debug.Log(id + " - " + score + " - " +  playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(uriWeb, form))
        {
            yield return www.SendWebRequest();

            string responseCode = (www.responseCode.ToString());

            if (www.result != UnityWebRequest.Result.Success){
                // Debug.Log(www.responseCode.ToString() + " - " + www.error);
            } else {
                string receivedText = www.downloadHandler.text;

                // JSON recebido
                JSONObject jsonValue = new JSONObject(receivedText);

                // Debug.Log(responseCode.ToString() + " - " + jsonValue.GetField("mensagem").ToString().Trim('"'));
            }
        }
    }

    // IEnumerator PutHighScore (int id, int score, string name) {

    //     // // Debug.Log(score + " - " + name);

    //     byte[] myData = System.Text.Encoding.UTF8.GetBytes("id=" + id + "&playername=" + name + "&score=" + score);
    //     using (UnityWebRequest www = UnityWebRequest.Put("http://localhost/sqlconnect/highscore.php/", myData))
    //     {
    //         yield return www.SendWebRequest();

    //         if (www.result != UnityWebRequest.Result.Success)
    //         {
    //             // Debug.Log(www.error);
    //         }
    //         else
    //         {                
    //             string jsonRecieved = www.downloadHandler.text;
    //             // Debug.Log(jsonRecieved);
    //         }
    //     }
    // }
}