using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/*
### - INFO DOCMENT

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 14/07/2021

    Atualizações: 
        15/07/2021: adição da tratativa que define se os dados utilizados serão locais ou via WEBRequest.

    Descrição:  
        Classe que possui métodos e objetos que são utilizados localmente e em outros scripts.
        Video Aula: https://youtu.be/iAbaqGYdnyI

    Variáveis:    
        private Transform entryContainer;
        
        private Transform entryTemplate;
        
        private List<HighscoreEntry> highscoreEntryListNew;

        private List<Transform> highscoreEntryTransformList;
        
        public GameOverScreen GameOverScreen;
    Métodos:
        Awake(): Ao "acordar" o gameObject ao qual esse script está atrelado, ele irá fazer uma série de tratativas para consultar o RANK 
                na tabela "highscoresTable" no PlayerPrefs. Ao fim chama o método CreateHighscoreEntryTransform para cada linha encontrada.
        
        CreateHighscoreEntryTransform():

    Classes:

###*/

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;    
    private Transform entryTemplateClone;

    // List que contém SCORE e NAME
    private List<HighscoreEntry> highscoreEntryListNew;    
    private List<Transform> highscoreEntryTransformList;
    public GameOverScreen GameOverScreen;
    public GameObject LoadingIcon, highScoreFirstButton;

    private static JSONObject jsonScores;

    // Receive this Value from IEnumerator "GetHighScore" from HighScoreRegistration script. It's used ahead in the code (ONLY WEB REQUEST)
    public static void receiveDataAll(JSONObject jsonScoresParameter){
        jsonScores = jsonScoresParameter;   
    }

    private void OnEnable() 
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(highScoreFirstButton);   

        LoadingIcon.gameObject.SetActive(true);
        StartCoroutine(ProcessData());        
    }

    IEnumerator ProcessData(){
        entryContainer = transform.Find("highscoreEnterContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);        
                
        Highscores highscores;
        List<HighscoreEntry> highscoreEntryListDatabase;
        
        // Call IEnumerator "GetHishScore" from "HighScoreRegistration" Script        
        string typeSeek = "allData";        // NO FILTERS ON BACK-END
        FindObjectOfType<HighScoreRegistration>().CallGetHighScore(typeSeek);
        
        // Way I found to wait for response from the Server (it will cancel the request after 2 seconds)
        int y = 1;
        while(jsonScores == null)
        {
            yield return new WaitForSeconds(1f);
            y++;
            if (y > 15){
                // Debug.Log("Cancelando Requisição - #HighScoreTable.cs - IEnumerator ProcessData()");
                break;
            }
        }

        yield return new WaitForSeconds(1f);        

        if (jsonScores == null) {
            // Debug.Log("Erro na requisição - #HighScoreTable.cs - IEnumerator ProcessData()");
        } else {
            // Debug.Log("Sucesso na requisição - #HighScoreTable.cs - IEnumerator ProcessData()");
            highscoreEntryListDatabase = new List<HighscoreEntry>() {};

            highscores = new Highscores { highscoreEntryList = highscoreEntryListDatabase };

            // Debug.Log(jsonData.Count);
        
            for(int i = 0; i < jsonScores.list.Count; i++) {
                
                // Debug.Log(jsonScores[i].GetField("playername").ToString() + " - " + jsonScores[i].GetField("score").ToString());

                // Needed to format in the Righ way both of the Fiedls berfore anything
                
                int tmpId = int.Parse(jsonScores[i].GetField("id").ToString().Trim('"'));       // Deletes " in the both sides, after that Transforms in INT
                string tmpName = jsonScores[i].GetField("playername").ToString().Trim('"');     // Deletes " in the both sides
                int tmpScore = int.Parse(jsonScores[i].GetField("score").ToString().Trim('"')); // Deletes " in the both sides, after that Transforms in INT 


                HighscoreEntry highscoreEntry = 
                    new HighscoreEntry { 
                        score = tmpScore,
                        name = tmpName
                    };

                highscores.highscoreEntryList.Add(highscoreEntry);
            }    
            
            // Ordena do Maior ou Menor
            for (int i = 0; i < highscores.highscoreEntryList.Count; i++){
                for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++) {
                    if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score) {
                        // SWAP
                        HighscoreEntry tmp = highscores.highscoreEntryList[i];
                        highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                        highscores.highscoreEntryList[j] = tmp;
                    }
                }
            }

            LoadingIcon.gameObject.SetActive(false);
            highscoreEntryTransformList = new List<Transform>();
            
            int x = 0;
            foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList) {            
                x++;
                // Limita tabela de HighScores para 10 linhas
                if (!(x <= 10))
                    yield break;
                    CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList); 
            }

            // // Debug.Log(PlayerPrefs.GetString("highscoreTable"));
        }       
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        float templateHeight = 25f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank) {
            default:
                rankString = rank + "th"; break;

            case 1: rankString = "1st"; break;
            case 2: rankString = "2nd"; break;
            case 3: rankString = "3rd"; break;
        }

        entryTransform.Find("[Text] Position").GetComponent<TextMeshProUGUI>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("[Text] Score").GetComponent<TextMeshProUGUI>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("[Text] Name").GetComponent<TextMeshProUGUI>().text = name;

        // Line Background (par ou ímpar)
        // entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Destaca Player Atual
        if (name == PlayerPrefs.GetString("PlayerName", "")) {
            // entryTransform.Find("[Text] Position").GetComponent<TextMeshProUGUI>().color = Color.red;
            // entryTransform.Find("[Text] Position").GetComponent<TextMeshProUGUI>().fontWeight = FontWeight.Bold;
            // entryTransform.Find("[Text] Position").GetComponent<TextMeshProUGUI>().fontSize = 15;
            
            // entryTransform.Find("[Text] Score").GetComponent<TextMeshProUGUI>().color = Color.red;
            // entryTransform.Find("[Text] Score").GetComponent<TextMeshProUGUI>().fontWeight = FontWeight.Bold;
            // entryTransform.Find("[Text] Score").GetComponent<TextMeshProUGUI>().fontSize = 15;

            entryTransform.Find("[Text] Name").GetComponent<TextMeshProUGUI>().color = Color.red;
            entryTransform.Find("[Text] Name").GetComponent<TextMeshProUGUI>().fontWeight = FontWeight.Bold;            
        }

        // Forme de setar cores por HEXADECIMAL
        Color colorGold;
        Color colorSilver;
        Color colorBronze;

        ColorUtility.TryParseHtmlString("#FFD700", out colorGold);
        ColorUtility.TryParseHtmlString("#C0C0C0", out colorSilver);
        ColorUtility.TryParseHtmlString("#CD7F32", out colorBronze);

        // Set Trophy ACTIVE and COLOR
        switch (rank) {
            default: 
                entryTransform.Find("Trophy").gameObject.SetActive(false);
            break;
            case 1:
                entryTransform.Find("Trophy").gameObject.GetComponent<Image>().color = colorGold;
                break;
            case 2:
                entryTransform.Find("Trophy").gameObject.GetComponent<Image>().color = colorSilver;
                break;            
            case 3:
                entryTransform.Find("Trophy").gameObject.GetComponent<Image>().color = colorBronze;
                break;
        }

        transformList.Add(entryTransform);
    }

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    // Representa uma "fonte" de dados para gravação de um score

    [System.Serializable]
    private class HighscoreEntry {
        public int score;
        public string name;
    }

    public void BackButton() {        
        StartCoroutine(CloseTable());
    }

    IEnumerator CloseTable() {
        FindObjectOfType<AudioManager>().Play("[FX] SelectionConfirm");
        gameObject.GetComponent<Animator>().SetTrigger("CloseTable");

        yield return new WaitForSeconds(.5f); // Animation lenght

        // Desativa Tabela
        transform.gameObject.SetActive(false);

        // Destruo todos os Clones criados
        for (int i = 0; i < entryContainer.childCount; i++) {            
            if (i > 0){
                Destroy(entryContainer.transform.GetChild(i).gameObject);
            }
        }

        GameOverScreen.gameObject.SetActive(true);        
    }
}