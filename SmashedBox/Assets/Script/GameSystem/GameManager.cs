using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      It manages almost everything in the "Game" Scene
 *      
 *      Thsi game is based on this Tutorial: https://youtu.be/fuRxrDVnHMI?list=PLj51HEHhPH1OijNJLh4tBNd6PLoaOThra
 *
 * DATES
 *      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
 *   
*/

public class GameManager : MonoBehaviour
{
    // # External GameObjs
    public GameOverScreen GameOverScreen;
    public GameObject killerRock, playerCrate; // Killer, Player
    public GameObject NetWorkErroImg, NetWorkErroImgTwo, LoadingIcon, LoadingIconTwo;       // NetWorkError Icons & Loading Icons
    public GameObject PlayerHud;  // Boost Bar & Commands Help
           
    public Material materialDay , materialNight, materialBlend; // SkyBoxes    
    public Material CRMaterialNight; // CartoonRiver

    // # Levels Seconds
    private int levelOneEnds    = 50;
    private int levelTwoEnds    = 100;
    private int levelThreeEnds  = 150;
    private int levelFourEnds   = 210;

    // private int levelOneEnds    = 10;
    // private int levelTwoEnds    = 20;
    // private int levelThreeEnds  = 30;
    // private int levelFourEnds   = 40;    
    
    // Player Bloom
    public  Material redBloom, greenBloom, iceBlueBloom; // BloomBox
    public GameObject DirectionalLight, CartoonRiver, MoonObject; // DirectionalLight , IslandLight, Water, Moon    
    public List<GameObject> SpotLights = new List<GameObject>(); // SpotLights

    Color colorLevelTwo, colorDirectionalLight, colorAmbient;

    // UI Icons and Text
    public TextMeshProUGUI scoreText, highestScoreText, personalBestText, levelText;    
    private Animator animatorLevelText;
    private AnimationClip[] clips;
    
    // SongList
    private Hashtable audioListSFX = new Hashtable();
    private Hashtable audioListTHEMES = new Hashtable();
    public static string initialThemeOnGame;

    // Killer Properties
    private int maxKillerToSpawn;
    private float maxDrag;

    public static int score;
    private float timer;
    
    // Server / Database
    private int highestScoreTMP, personalBestTMP, idTMP;
    private bool isConnectedServer = false;
    private static JSONObject jsonReceivedData; // WEB REQUEST - JSON Scores
    
    // GameOver / Levels
    private static bool gameOver;   
    public static bool gameOverScreenShowed, levelOneisRunning, levelTwoisRunning, levelThreeisRunning, levelFourisRunning, levelFinalisRunning;    

    // Hit Rock Stone
    public GameObject hitTextOne, hitTextTwo;
    private static bool comboHit = false;


    // Called by "HighScoreRegistration" and recieves DataBase data (Score)
    public static void receiveDataGameManager(JSONObject jsonData){
        jsonReceivedData = jsonData; // Highest Score + Personal Score (2 lines, if it is the same, 1 line)
    }
    
    private void Start() {
        // LevelText Animator
        animatorLevelText   = levelText.gameObject.GetComponent<Animator>(); // Cashing Animator
        clips               = animatorLevelText.runtimeAnimatorController.animationClips;
    }

    // ### CODE STARTS
    void OnEnable()
    {
        if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0)
        {
            audioListTHEMES.Clear(); // Clear everytime enable that GameObj the game

            // Firts Songs
            audioListTHEMES.Add(1 , "[TK] Theme on Game");
            audioListTHEMES.Add(2 , "[TK] Monkey Warhol");           
            
            int sortThemeSong = Random.Range(1, 3);
            initialThemeOnGame = audioListTHEMES[sortThemeSong].ToString();
            
            if (audioListTHEMES.Contains(sortThemeSong)) {
                FindObjectOfType<AudioManager>().FadeAudio(initialThemeOnGame, 1f, true); // Fade In
            }
        }

        gameOver                = false;
        gameOverScreenShowed    = false;
        levelOneisRunning       = false;
        levelTwoisRunning       = false;
        levelThreeisRunning     = false;
        levelFourisRunning      = false;
        
        // Por Default - Level 01
        maxKillerToSpawn = 1    ;
        maxDrag = 4             ;

        StartCoroutine(SpawnKillers());
        StartCoroutine(HighestScoreValue()); 

        audioListSFX.Clear(); // Clear to use again

        // Level 1 - Play After Born
        audioListSFX.Add(1 , "[FX] YehawScream");
        audioListSFX.Add(2 , "[FX] Eazy");
        audioListSFX.Add(3 , "[FX] BadFelling");
    }

    private void Update() {
        if (gameOver) {              
            if (!gameOverScreenShowed) {
                GameOverManage(); // Método de Ativação da SCREEN GAMEOVER            
            }
            return;
        }

        // DeltaTime é o tempo que levou para entrar em outro FRAME (Se o jogo está em 60 FPS, DeltaTime = 1/60)
        // É uma maneira de calcular tempo independente da performance do nosso computador
        if (!comboHit) {   // Animação HitStone, não conta Score
            timer += Time.deltaTime;
            if (timer >= 1f){
                if (levelText.transform.gameObject.activeSelf)                
                    return;            
                
                score++;
                scoreText.text = score.ToString();

                timer = 0;

                // Highest Score
                if (score > highestScoreTMP && isConnectedServer) {
                    highestScoreText.text = score.ToString();
                }

                // Personal Best
                if (score > personalBestTMP && isConnectedServer) {
                    personalBestText.text = score.ToString();
                }
            }            
        }        
    }

    private void GameOverManage() {
        GameOverScreen.Setup(score, PlayerPrefs.GetString("PlayerName"));                
        gameOverScreenShowed    = true;

        levelText.transform.gameObject.SetActive(false);
        PlayerHud.gameObject.SetActive(false);
        FindObjectOfType<AudioManager>().Play("[FX] WoodenCrateDestruction"); //  Wooden Box Smashed

        // Sound Design
        if (personalBestTMP < score) {
            FindObjectOfType<AudioManager>().Play("[FX] WowVoice");
        } else {
            int sortBornFX;
            if (levelOneisRunning){                    
                // Level 1 - Play After Die
                audioListSFX.Add(10 , "[FX] GameOverLaught");
                audioListSFX.Add(11 , "[FX] ActorLaught");
                audioListSFX.Add(12 , "[FX] WTFLaught");
                
                sortBornFX = Random.Range(10, 13); // 10 a 12 - Sort para Audio Death
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString()); 
            } else if(levelTwoisRunning ^ levelThreeisRunning) {
                // Level 2/3 - Play After Die
                audioListSFX.Add(20 , "[TK] SadPiano");
                audioListSFX.Add(21 , "[FX] DoitJustDo");
                audioListSFX.Add(22 , "[FX] No God");
            
                sortBornFX = Random.Range(20, 23); // 20 a 22 - Sort para Audio Death
                
                if (sortBornFX == 20 && PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0){ // SadViolin & Music is On
                    if (score <= levelTwoEnds && !(initialThemeOnGame == ""))
                        FindObjectOfType<AudioManager>().FadeAudio(initialThemeOnGame, 1f, false); // Fade Out
                    else
                        FindObjectOfType<AudioManager>().FadeAudio("[TK] AreYouReadyDubStep", 1f, false); // Fade Out
                }

                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString());
            } else if (levelFinalisRunning ^ levelFourisRunning) {
                // Level 4 - Play After Die
                audioListSFX.Add(30 , "[FX] WowVoice");
                audioListSFX.Add(31 , "[FX] Claps");
                audioListSFX.Add(32 , "[FX] Yaaay");

                sortBornFX = Random.Range(30, 33); // 30 a 32 - Sort para Audio Death
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString()); 
            }
        }

        // Clear Bool variables
        levelOneisRunning       = false;
        levelTwoisRunning       = false;
        levelThreeisRunning     = false;
        levelFourisRunning      = false;        
        levelFinalisRunning     = false;
    }        

    private IEnumerator HighestScoreValue(){

        string typeSeek = PlayerPrefs.GetString("PlayerName", "");       

        // Call IEnumerator "GetHishScore" from "HighScoreRegistration" Script
        FindObjectOfType<HighScoreRegistration>().CallGetHighScore(typeSeek);

        // Way I found to wait for response from the Server (it will cancel the request after 2 seconds)
        int y = 1;
        while(jsonReceivedData == null)
        {
            LoadingIcon.gameObject.SetActive(true);
            LoadingIconTwo.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            y++;
            if (y > 15){
                isConnectedServer = false; // Confiramando inexistência de Conexão
                // Debug.Log("Cancelando Requisição - #GameManager.cs - HighestScoreValue()");
                // ######### PARAR O JOGO E INFORMAR USUÁRIO DO PROBLEMA ENCONTRADO
                LoadingIcon.gameObject.SetActive(false);
                LoadingIconTwo.gameObject.SetActive(false);

                NetWorkErroImg.SetActive(true);
                NetWorkErroImgTwo.SetActive(true);                                
                break;
            }
        }
        
        if (jsonReceivedData != null){
            isConnectedServer = true;

            // CAPTURA VALORES DO JSON
            highestScoreTMP = jsonReceivedData[0].GetField("score") == null ? 0 : int.Parse(jsonReceivedData[0].GetField("score").ToString().Trim('"'));           

            // CASO SO PLAYER ATUAL SEJA TAMBÉM O TOP 1
            if (PlayerPrefs.GetString("PlayerName", "") == jsonReceivedData[0].GetField("playername").ToString().Trim('"'))
            {
                personalBestTMP     = highestScoreTMP;
                idTMP               = jsonReceivedData[0] == null ? 0 : int.Parse(jsonReceivedData[0].GetField("id").ToString().Trim('"'));
            } 
            else 
            {
                personalBestTMP     = jsonReceivedData[1] == null ? 0 : int.Parse(jsonReceivedData[1].GetField("score").ToString().Trim('"'));
                idTMP               = jsonReceivedData[1] == null ? 0 : int.Parse(jsonReceivedData[1].GetField("id").ToString().Trim('"'));
            }

            // Preenche UI Values
            highestScoreText.text = highestScoreTMP.ToString();
            personalBestText.text = personalBestTMP.ToString();

            // Grava temporariamente o SCORE e ID do player (SERÃO USADOS EM GAMEOVERSCREE.CS - SETUP())
            if (PlayerPrefs.GetInt("PlayerID", 0) != 0) {
                PlayerPrefs.DeleteKey("PlayerID");
            }
            PlayerPrefs.SetInt("PlayerID", idTMP);


            if (PlayerPrefs.GetInt("PlayerPersonalBest", 0) != 0) 
            {
                PlayerPrefs.DeleteKey("PlayerPersonalBest");
            }
            PlayerPrefs.SetInt("PlayerPersonalBest", personalBestTMP);

            // Atualiza UI
            LoadingIcon.gameObject.SetActive(false);
            LoadingIconTwo.gameObject.SetActive(false);
        }
        // "Destrói" Objeto        
        jsonReceivedData = null;
    }

    private IEnumerator SpawnKillers(){
        // Pós GameOver
        if (gameOverScreenShowed){
            maxKillerToSpawn = 3;
            maxDrag = 3;
        } else {            
            if (score < levelOneEnds && !levelOneisRunning)                                         // LEVEL 01
            {
                StartCoroutine(LevelManager(1));
                yield return new WaitForSeconds(2.5f); // 2.5 Seconds to START droping Stones
            }
            else if (score >= levelOneEnds && score < levelTwoEnds && !levelTwoisRunning)           // LEVEL 02
            {
                StartCoroutine(LevelManager(2));
                yield return new WaitForSeconds(2.5f); // 2.5 Seconds to START droping Stones
            } 
            else if (score >= levelTwoEnds && score < levelThreeEnds && !levelThreeisRunning)       // LEVEL 03
            {
                StartCoroutine(LevelManager(3));
                yield return new WaitForSeconds(2.5f); // 2.5 Seconds to START droping Stones
            } 
            else if ((score >= levelThreeEnds && score < levelFourEnds) && !levelFourisRunning)     // LEVEL 04
            {
                StartCoroutine(LevelManager(4));
                yield return new WaitForSeconds(2.5f); // 2.5 Seconds to START droping Stones
            } 
            else if (score >= levelFourEnds && !levelFinalisRunning)                  // LEVEL 05
            {
                StartCoroutine(LevelManager(5));
                yield return new WaitForSeconds(2.5f); // 2.5 Seconds to START droping Stones
            }            
        }
       
        var killerToSpawn = Random.Range(1, maxKillerToSpawn + 1);
        
        int[] intArrayX = new int[killerToSpawn];

        for (int i = 0; i < killerToSpawn; i++)
        {            
            // Eixo X a ser iniciado
            var eixoX = Random.Range(-7, 7);

            // A partir do 2º processamente, evita spawn de duas caixas no mesmo EIXO
            if ((i > 0)) {
                foreach (int x in intArrayX){
                    if (x == eixoX){
                        eixoX = Random.Range(-7, 8);
                        foreach (int y in intArrayX){
                            if (y == eixoX){
                                eixoX = Random.Range(-7, 8);
                            }
                        }
                    }
                }
            }

            // intArrayX.SetValue(i, i); -  Poderia usar desta forma
            intArrayX[i] = eixoX;
            
            // Instancia o Objeto (Retorna um GamboObject)
            var killer = Instantiate(killerRock, new Vector3(eixoX, 10, -5), Quaternion.identity);

            if (gameOverScreenShowed) {
                killer.GetComponent<Rigidbody>().drag = maxDrag;
            } else {
                if (levelOneisRunning) 
                {
                    killer.GetComponent<Rigidbody>().drag = maxDrag;
                } else if (levelTwoisRunning) 
                {
                    var dragVar = Random.Range(2f, maxDrag+1);                    
                    killer.GetComponent<Rigidbody>().drag = dragVar; // Beetween 2 - 3
                } else if (levelThreeisRunning)
                {
                    var dragVar = Random.Range(1f, maxDrag+1);                    
                    killer.GetComponent<Rigidbody>().drag = dragVar; // Beetween 1 - 2
                } else if (levelFourisRunning)
                {
                    var dragVar = Random.Range(1f, maxDrag+1);                    
                    killer.GetComponent<Rigidbody>().drag = dragVar; // Beetween 0 - 1
                } else if (levelFinalisRunning)
                {                    
                    var dragVar = Random.Range(.1f, maxDrag+1);
                    // Debug.Log("2 - MaxDrag: " + maxDrag + " - DragVar: " + dragVar);
                    killer.GetComponent<Rigidbody>().drag = dragVar;
                }
            }
        }

        // "Destruo" o Array após o FOR
        intArrayX = null;

        yield return new WaitForSeconds(1f);        

        yield return SpawnKillers();
    }

    private IEnumerator LevelManager(int levelNumber){        
        int sortBornFX;
        Light DirLightComponent = DirectionalLight.gameObject.GetComponent<Light>();        

        yield return new WaitForSeconds(0.01f);
        FindObjectOfType<AudioManager>().Play("[FX] DrumsLevel");
        FindObjectOfType<AudioManager>().Play("[FX] LevelCut");

        switch (levelNumber)
        {
            // LEVEL 01
            case 1:
                levelOneisRunning   = true      ;            
                maxKillerToSpawn    = 1         ;
                maxDrag             = 3         ;
                levelText.text      = "LEVEL 1" ;                                
                
                sortBornFX = Random.Range(1, 4); // 1 a 3 - Sort initial SFX
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString())     ; 

                FindObjectOfType<AudioManager>().Play("[FX] BirdsSinging")   ;
                FindObjectOfType<AudioManager>().Play("[FX] WindSound")      ;
                FindObjectOfType<AudioManager>().Play("[FX] WaterSound")     ;           
                
                // Debug.Log("LEVEL 01: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxKillerToSpawn + " - #GameManager.cs - Update()");    
                break;
            case 2: // LEVEL 02
                levelOneisRunning   = false     ;
                levelTwoisRunning   = true      ;
                maxKillerToSpawn    = 2         ;
                maxDrag             = 3         ;
                levelText.text      = "LEVEL 2" ;

                // Change SKYBOX and DIRECTIONAL LIGHT
                RenderSettings.skybox = materialNight;
                // RenderSettings.fog = false;

                ColorUtility.TryParseHtmlString("#F8AB79", out colorLevelTwo);
                DirLightComponent.color = colorLevelTwo;            

                // Debug.Log("LEVEL 02: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxKillerToSpawn + " - #GameManager.cs - Update()"); 
                break;
            case 3: // LEVEL 03
                levelTwoisRunning   = false     ;
                levelThreeisRunning = true      ;
                maxKillerToSpawn    = 2         ;
                maxDrag             = 2         ;
                levelText.text      = "LEVEL 3" ;

                yield return new WaitForSeconds(.1f);

                FindObjectOfType<AudioManager>().Play("[FX] BigLightwitch"); // Lights Turn On SFX

                if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0) // If Music is turned On, Changes the Song
                {
                    FindObjectOfType<AudioManager>().FadeAudio(initialThemeOnGame, 1f, false); // Fade Out

                    FindObjectOfType<AudioManager>().FadeAudio("[TK] AreYouReadyDubStep", 1f, true); // Fade In
                }            

                ColorUtility.TryParseHtmlString("#FFFFFF", out colorDirectionalLight);
                ColorUtility.TryParseHtmlString("#729598", out colorAmbient);

                // Change SKYBOX and DIRECTIONAL LIGHT
                RenderSettings.skybox           = materialBlend; 
                RenderSettings.fogColor         = colorAmbient;
                RenderSettings.ambientSkyColor  = colorAmbient;

                playerCrate.GetComponent<Renderer>().material = redBloom; // Player Bloom

                DirLightComponent.intensity = 0;
                DirLightComponent.color = colorDirectionalLight;

                // CartoonRiver.gameObject.GetComponent<MeshRenderer>().material = CRMaterialNight; // Water turns Darker                
                
                // Enable all SpotLight Lights
                for (int i = 0; i < SpotLights.Count; i++) {
                    SpotLights[i].gameObject.SetActive(true);
                }

                // Debug.Log("LEVEL 03: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxKillerToSpawn + " - #GameManager.cs - Update()");                    
                break;
            case 4: // LEVEL 04
                levelThreeisRunning     = false     ;
                levelFourisRunning      = true      ;                
                maxKillerToSpawn        = 3         ;
                maxDrag                 = 2         ;
                levelText.text          = "LEVEL 4" ;

                yield return new WaitForSeconds(.1f);

                ColorUtility.TryParseHtmlString("#3F3F3F", out colorDirectionalLight);
                ColorUtility.TryParseHtmlString("#000000", out colorAmbient);

                // LIGHTNING -> ENVIROMENT
                RenderSettings.skybox           = materialNight;    // SkyBox
                RenderSettings.fogColor         = colorAmbient;     // FogColoer
                RenderSettings.ambientSkyColor  = colorAmbient;     // AmbientColor

                playerCrate.GetComponent<Renderer>().material = greenBloom; // Player Bloom

                // DIRECTIONAL LIGHT
                DirLightComponent.intensity = .1f;
                DirLightComponent.color = colorDirectionalLight;            

                // Debug.Log("LEVEL 04: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxKillerToSpawn + " - #GameManager.cs - Update()");    
                break;
            case 5: // LEVEL 05                
                levelFourisRunning  = false         ;
                levelFinalisRunning = true          ;
                maxKillerToSpawn    = 3             ;
                maxDrag             = 2             ;
                levelText.text      = "FINAL LEVEL" ;

                yield return new WaitForSeconds(.1f);

                ColorUtility.TryParseHtmlString("#616161", out colorAmbient);

                // LIGHTNING -> ENVIROMENT
                RenderSettings.fogColor         = colorAmbient; // FogColor
                RenderSettings.ambientSkyColor  = colorAmbient; // AmbientColor
                            
                // DIRECTIONAL LIGHT
                DirectionalLight.gameObject.SetActive(false);
                
                MoonObject.gameObject.SetActive(true);  // Moon
                playerCrate.GetComponent<Renderer>().material = iceBlueBloom; // Player Bloom
                
                // Disable all SpotLight Lights (AllDark)
                for (int i = 0; i < SpotLights.Count; i++) {
                    SpotLights[i].gameObject.SetActive(false);
                }

                // Debug.Log("LEVEL 05: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxKillerToSpawn + " - #GameManager.cs - Update()");                 
                break;
            default:
                Debug.LogError("Nenhum parâmetro passado como LEVEL. IEnumerator 'LevelManager' - GameManager.cs");
                yield break;
        }                    

        // LevelText Animation        
        float waitTime = 0f;        
        foreach(AnimationClip clip in clips) {            
            if (clip.name == "LevelAnimation") {
                waitTime =+ clip.length;  // Tempo da animação
                break;
            }
        }
        
        levelText.transform.gameObject.SetActive(true)  ;
        yield return new WaitForSeconds(waitTime)       ;
        levelText.transform.gameObject.SetActive(false) ;        
    }

    public static void GameOver(){
        gameOver = true;
    }

    public void DestroyedStone(Transform transoformRock){ // Called from Killer.cs script
        StartCoroutine(HitTextSpawn(transoformRock));        
    }

    IEnumerator HitTextSpawn(Transform transoformRock) {
        int numbDisplay     = 1;
        var tranformText    = new Vector3(transoformRock.position.x, transoformRock.position.y + 2f, transoformRock.position.z);
        var textMeshOne     = hitTextOne.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        var textMeshTwo     = hitTextTwo.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();        
        
        FindObjectOfType<AudioManager>().Play("[FX] HitTextPopUp");
        
        while (!(numbDisplay > 10) && !gameOver) {                        

            if (numbDisplay == 10 && !gameOver) {
                textMeshTwo.SetText(numbDisplay.ToString() + "+ Score!");
                Instantiate(hitTextTwo, tranformText, Quaternion.identity);                

                var animator = scoreText.transform.GetComponent<Animator>();

                yield return new WaitForSeconds(1.3f); // Animation Lenght of HitText2
                comboHit = true;// Stops Normal Score Count

                if (!gameOver) {
                    score = score + numbDisplay;
                    scoreText.text = score.ToString();

                    animator.SetTrigger("HitIncrease");
                    yield return new WaitForSeconds(1f); // Animation lenght of ScoreValue (started on the line above)                    
                }

                comboHit = false; // Continue Normal Score Count
                break;         
            }

            textMeshOne.SetText(numbDisplay.ToString() + "+ Score!");
            Instantiate(hitTextOne, tranformText, Quaternion.identity);            
            numbDisplay = numbDisplay + 1;
            yield return new WaitForSeconds(.07f);            
        }
       
        yield return null;
    }
}