/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Classe que possui métodos e objetos que são utilizados localmente e em outros scripts.
 *      Video Aula: https://youtu.be/iAbaqGYdnyI
 *
 * DATES
 *      14/07/2021 - Creation of script
 *      15/07/2021 - Adição da tratativa que define se os dados utilizados serão locais ou via WEBRequest.
 *      05/04/2024 - Start of the adaptation to persist data localy (PlayerPrefs)
 *      08/04/2024 - Continuation of the changes to offline listing scores. Adaptation to use JSON forma storing in string format on PlayerPrefs.
 *
 * CLASSES
 *      HighscoreEntry() - Represents ONE entry of Score record. It's Serializable
 * 
 *      HighscoreList() - List of HighscoreEntry objects
 *
 * METHODS:
 *      Awake() 
 *          Caches the UI elements to be used on the Table 
 *          Sorts the Score entries to be Descending
 *          Calls CreateHighscoreEntryTransform to Instantiate each line of the Table
 *      
 *      OnEnable()
 *          Sets the Selected Button
 *      
 *      CreateHighscoreEntryTransform()
 *          Instantiates one line on the table each with different requisites
 *      
 *      public GetAllScores()
 *          Returns a object of tipy Highscores (list of highscore entries) from PlayerPrefs (string JSON)
 *          It's calleed in GameOverScreen.Setup()
 *      
 *      AddHighscoreEntry()
 *          It reads a new score data, parses it into JSON, then saves it in PlayerPrefs
 *      
 *      BackButton() / CloseTable(): ...
 *
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class HighScoreTable : MonoBehaviour
{
    public GameOverScreen GameOverScreen;
    public GameObject highScoreFirstButton;

    // Reference - Render each line properly
    private Transform entryContainer;
    private Transform entryTemplate;
    
    private List<HighscoreEntry> highscoreEntryList; // Used for hardcoded Tests only
    private List<Transform> highscoreEntryTransformList;    // ???

    public const string highscoreListKey = "HighScoreJson"; // PlayerPrefs Key

    [System.Serializable] // It must be Serializable ir order to be converted to JSON
    public class HighscoreEntry {
        public int score;
        public string name;
    }

    public class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    private void Awake()
    {
        entryContainer  = transform.Find("highscoreEnterContainer");
        entryTemplate   = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        // Test Obj        
        /*highscoreEntryList = new List<HighscoreEntry>()
        {
            new HighscoreEntry { score = 516, name = "ADAM"},
            new HighscoreEntry { score = 546, name = "ALEX"},
            new HighscoreEntry { score = 854, name = "JOEL"}
        };*/


        Highscores highscores = this.GetAllScores();        

        highscoreEntryTransformList = new List<Transform>();

        // Inatantiate Lines
        int x = 0;
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            x++;
            if (!(x <= 10)) break; // Limite de 10 linhas
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
        
        //Highscores highscores = new Highscores() { highscoreEntryList = highscoreEntryList };
        //string jsonString = JsonUtility.ToJson(highscores);
        //PlayerPrefs.SetString(highscoreListKey, jsonString);
        //PlayerPrefs.Save();
    }


    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null); //clear selected object
        EventSystem.current.SetSelectedGameObject(highScoreFirstButton); //set a new selected object
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) 
    {
        float templateHeight = 25f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
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
        if (name == PlayerPrefs.GetString("PlayerName", ""))
        {
            entryTransform.Find("[Text] Name").GetComponent<TextMeshProUGUI>().color = Color.red;
            entryTransform.Find("[Text] Name").GetComponent<TextMeshProUGUI>().fontWeight = FontWeight.Bold;
        }

        // Cores por HEXADECIMAL
        Color colorGold;
        ColorUtility.TryParseHtmlString("#FFD700", out colorGold);

        Color colorSilver;
        ColorUtility.TryParseHtmlString("#C0C0C0", out colorSilver);
        
        Color colorBronze;
        ColorUtility.TryParseHtmlString("#CD7F32", out colorBronze);

        // Set Trophy ACTIVE and COLOR
        switch (rank)
        {
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

    public Highscores GetAllScores() 
    {
        Highscores highscores = new Highscores() { };

        if (PlayerPrefs.HasKey(highscoreListKey))
        {
            string jsonString = PlayerPrefs.GetString(highscoreListKey);
            highscores = JsonUtility.FromJson<Highscores>(jsonString);

            // Sort Entry by Score (Descending)
            for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
            {
                for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
                {
                    if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                    {
                        // SWAP
                        HighscoreEntry tmp = highscores.highscoreEntryList[i];
                        highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                        highscores.highscoreEntryList[j] = tmp;
                    }
                }
            }
        }

        return highscores;
    }

    public void AddHighscoreEntry(int score, string playerName)
    {
        HighscoreEntry highscoreEntry = new() { score = score, name = playerName };
        
        Highscores highscores;
        string jsonString;
        
        if (PlayerPrefs.HasKey(highscoreListKey)) // Load Saved Highscores (if there's any)
        {
            highscores = this.GetAllScores();
            
            if ( highscores.highscoreEntryList.Find(x => (x.name == playerName)) == null) // Insert First Score
            {
                highscores.highscoreEntryList.Add(highscoreEntry); // Add new Entry to Highscores Obj      
            }
            else // Updates Score
            {
                highscores.highscoreEntryList.Find(x => (x.name == playerName)).score = score;
            }
        }
        else  // Empty List - First Score Ever
        {
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() { highscoreEntry } };
        }

        // Save it in Playerprefs        
        jsonString = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(highscoreListKey, jsonString);
        PlayerPrefs.Save();        

    }

    public void BackButton() {
        StartCoroutine(CloseTable());
    }

    private IEnumerator CloseTable() 
    {
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