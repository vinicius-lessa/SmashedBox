/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Manage UI information
 *      Get/Set Player's Personal Best Score
 *      Spawn Stones for the player to avoid
 *      Enables GameOver Screen 
 *      Manage Level Shift
 *
 *      Based on Tutorial: https://youtu.be/fuRxrDVnHMI
 *
 * DATES
 *      19/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Start of changes to disable online scoreboarding and connectivity check
 *   
*/

/*  
CHECK / DELETE
    NetWorkErroImg
    NetWorkErroImgTwo
    LoadingIcon
    LoadingIconTwo
    highestScoreTMP
    personalBestTMP
    idTMP
    isConnectedServer
    ParsePlayerScore
    InternetConnection
    PlayerID
*/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // # External GameObjs
    public GameOverScreen GameOverScreen;
    public GameObject StonePrefab, PlayerObj; // Killer, Player
    public GameObject NetWorkErroImg, NetWorkErroImgTwo, LoadingIcon, LoadingIconTwo;       // NetWorkError Icons & Loading Icons
    public GameObject PlayerHud;  // Boost Bar & Commands Help
    public GameObject ScoreUI;
    public GameObject WelcomeScreen;

    // # Materials
    public Material materialDay , materialNight, materialBlend; // SkyBoxes    
    public Material CRMaterialNight; // CartoonRiver

    // # Levels Shift Time
    private int levelOneEnds    = 50;
    private int levelTwoEnds    = 100;
    private int levelThreeEnds  = 150;
    private int levelFourEnds   = 210;
    
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

    // Stones Properties
    private int maxStonesToSpawn;
    private float maxDrag;

    // Other Properties
    public static int score;
    private float timer;
    string playerName;

    // Players ScoreBoard Data
    private int highestScoreTMP, personalBestTMP, idTMP;
    
    // GameOver / Levels
    private static bool gameOver;   
    public static bool gameOverScreenShowed, levelOneisRunning, levelTwoisRunning, levelThreeisRunning, levelFourisRunning, levelFinalisRunning;    

    // Stone Destruction
    public GameObject hitTextOne, hitTextTwo;
    private static bool comboHit = false;

    private GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable() 
    {
        score = 0;

        // LevelText Animator
        animatorLevelText   = levelText.gameObject.GetComponent<Animator>(); // Cashing Animator
        clips               = animatorLevelText.runtimeAnimatorController.animationClips;

        // bool startGame  = false;
        playerName = PlayerPrefs.GetString("PlayerName", "").ToString();

        // Check if There's a Player Logged
        if (string.IsNullOrEmpty(playerName)) {
            WelcomeScreenCall();
        } 
        else {
            StartGame();
        }
    }

    private void WelcomeScreenCall()
    {
        WelcomeScreen.SetActive(true);
        instance.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        // Activates Essential Objects
        PlayerObj.gameObject.SetActive(true);
        ScoreUI.SetActive(true);
        PlayerHud.gameObject.SetActive(true);

        // Initializes Gameplay bool Variables
        gameOver                = false;
        gameOverScreenShowed    = false;
        levelOneisRunning       = false;
        levelTwoisRunning       = false;
        levelThreeisRunning     = false;
        levelFourisRunning      = false;

        // Level 01
        maxStonesToSpawn = 1;
        maxDrag = 4;

        // UI Score Data
        personalBestTMP = GetPlayerHighestScore(playerName);
        personalBestText.text = personalBestTMP.ToString();

        highestScoreTMP = GetAllTimeHighestScore();
        highestScoreText.text = highestScoreTMP.ToString();

        StartCoroutine(InstantiateStone());        
    }

    void Update() {
        if (gameOver) {              
            if (!gameOverScreenShowed) {
                GameOverManager(); // Método de Ativação da SCREEN GAMEOVER            
            }
            return;
        }
        
        if (!comboHit) {   // Animação HitStone, não conta Score
            timer += Time.deltaTime;
            if (timer >= 1f){
                if (levelText.transform.gameObject.activeSelf)                
                    return;            
                
                score++;
                scoreText.text = score.ToString();

                timer = 0;

                // Highest Score
                if (score > highestScoreTMP)
                    highestScoreText.text = score.ToString();                

                // Personal Best
                if (score > personalBestTMP)
                    personalBestText.text = score.ToString();                
            }            
        }        
    }

    public static void GameOver() // Called by Player.cs when player dies
    { 
        gameOver = true;
    }

    void GameOverManager() 
    {
        bool newPersonalBest;

        gameOverScreenShowed = true;
        levelText.transform.gameObject.SetActive(false);
        PlayerHud.gameObject.SetActive(false);
                
        FindObjectOfType<AudioManager>().Play("[FX] WoodenCrateDestruction"); //  Wooden Box Smashed        

        // AUDIO
        if (score > personalBestTMP) { // New Personal Best
            newPersonalBest = true;

            FindObjectOfType<AudioManager>().Play("[FX] WowVoice");
        } 
        else {
            newPersonalBest = false;
            int sortBornFX;
            if (levelOneisRunning){                    
                // Level 1 - Play After Die
                audioListSFX.Add(10 , "[FX] GameOverLaught");
                audioListSFX.Add(11 , "[FX] ActorLaught");
                audioListSFX.Add(12 , "[FX] WTFLaught");
                
                sortBornFX = Random.Range(10, 13); // 10 a 12 - Sort para Audio Death
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString()); 
            } 
            else if(levelTwoisRunning ^ levelThreeisRunning) {
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
            } 
            else if (levelFinalisRunning ^ levelFourisRunning) {
                // Level 4 - Play After Die
                audioListSFX.Add(30 , "[FX] WowVoice");
                audioListSFX.Add(31 , "[FX] Claps");
                audioListSFX.Add(32 , "[FX] Yaaay");

                sortBornFX = Random.Range(30, 33); // 30 a 32 - Sort para Audio Death
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString()); 
            }
        }

        GameOverScreen.Setup(score, newPersonalBest, PlayerPrefs.GetString("PlayerName"));

        // Clear Level State
        levelOneisRunning       = false;
        levelTwoisRunning       = false;
        levelThreeisRunning     = false;
        levelFourisRunning      = false;
        levelFinalisRunning     = false;
    }        

    private int GetPlayerHighestScore(string playerName)
    {
        int score = PlayerPrefs.GetInt("PlayerPersonalBest", 0);
        return score;
    }

    private int GetAllTimeHighestScore()
    {
        int score = PlayerPrefs.GetInt("PlayerPersonalBest", 0);
        return score;
    }

    private IEnumerator InstantiateStone(){
        // Pós GameOver
        if (gameOverScreenShowed)
        {
            maxStonesToSpawn = 3;
            maxDrag = 3;
        } 
        else 
        {
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
       
        var killerToSpawn = Random.Range(1, maxStonesToSpawn + 1);
        
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
            var killer = Instantiate(StonePrefab, new Vector3(eixoX, 10, -5), Quaternion.identity);

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

        yield return InstantiateStone();
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
                maxStonesToSpawn    = 1         ;
                maxDrag             = 3         ;
                levelText.text      = "LEVEL 1" ;

                audioListSFX.Clear(); // Clear to use again

                // Level 1 - Play After Born
                audioListSFX.Add(1, "[FX] YehawScream");
                audioListSFX.Add(2, "[FX] Eazy");
                audioListSFX.Add(3, "[FX] BadFelling");

                sortBornFX = Random.Range(1, 4); // 1 a 3 - Sort initial SFX
                FindObjectOfType<AudioManager>().Play(audioListSFX[sortBornFX].ToString());

                // AUDIO
                if (PlayerPrefs.GetInt("MuteMusicToogle", 0) == 0)
                {
                    audioListTHEMES.Clear(); // Clear everytime enable that GameObj the game

                    // Firts Songs
                    audioListTHEMES.Add(1, "[TK] Theme on Game");
                    audioListTHEMES.Add(2, "[TK] Monkey Warhol");

                    int sortThemeSong = Random.Range(1, 3);
                    initialThemeOnGame = audioListTHEMES[sortThemeSong].ToString();

                    if (audioListTHEMES.Contains(sortThemeSong))
                        FindObjectOfType<AudioManager>().FadeAudio(initialThemeOnGame, 1f, true);
                }

                FindObjectOfType<AudioManager>().Play("[FX] BirdsSinging");
                FindObjectOfType<AudioManager>().Play("[FX] WindSound");
                FindObjectOfType<AudioManager>().Play("[FX] WaterSound");
                
                // Debug.Log("LEVEL 01: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxStonesToSpawn + " - #GameManager.cs - Update()");
                break;
            case 2: // LEVEL 02
                levelOneisRunning   = false     ;
                levelTwoisRunning   = true      ;
                maxStonesToSpawn    = 2         ;
                maxDrag             = 3         ;
                levelText.text      = "LEVEL 2" ;

                RenderSettings.skybox = materialNight; // Change SKYBOX and DIRECTIONAL LIGHT

                ColorUtility.TryParseHtmlString("#F8AB79", out colorLevelTwo);
                DirLightComponent.color = colorLevelTwo;            

                // Debug.Log("LEVEL 02: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxStonesToSpawn + " - #GameManager.cs - Update()"); 
                break;
            case 3: // LEVEL 03
                levelTwoisRunning   = false     ;
                levelThreeisRunning = true      ;
                maxStonesToSpawn    = 2         ;
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

                PlayerObj.GetComponent<Renderer>().material = redBloom; // Player Bloom

                DirLightComponent.intensity = 0;
                DirLightComponent.color = colorDirectionalLight;

                // CartoonRiver.gameObject.GetComponent<MeshRenderer>().material = CRMaterialNight; // Water turns Darker                
                
                // Enable all SpotLight Lights
                for (int i = 0; i < SpotLights.Count; i++) {
                    SpotLights[i].gameObject.SetActive(true);
                }

                // Debug.Log("LEVEL 03: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxStonesToSpawn + " - #GameManager.cs - Update()");                    
                break;
            case 4: // LEVEL 04
                levelThreeisRunning     = false     ;
                levelFourisRunning      = true      ;                
                maxStonesToSpawn        = 3         ;
                maxDrag                 = 2         ;
                levelText.text          = "LEVEL 4" ;

                yield return new WaitForSeconds(.1f);

                ColorUtility.TryParseHtmlString("#3F3F3F", out colorDirectionalLight);
                ColorUtility.TryParseHtmlString("#000000", out colorAmbient);

                // LIGHTNING -> ENVIROMENT
                RenderSettings.skybox           = materialNight;    // SkyBox
                RenderSettings.fogColor         = colorAmbient;     // FogColoer
                RenderSettings.ambientSkyColor  = colorAmbient;     // AmbientColor

                PlayerObj.GetComponent<Renderer>().material = greenBloom; // Player Bloom

                // DIRECTIONAL LIGHT
                DirLightComponent.intensity = .1f;
                DirLightComponent.color = colorDirectionalLight;            

                // Debug.Log("LEVEL 04: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxStonesToSpawn + " - #GameManager.cs - Update()");    
                break;
            case 5: // LEVEL 05                
                levelFourisRunning  = false         ;
                levelFinalisRunning = true          ;
                maxStonesToSpawn    = 3             ;
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
                PlayerObj.GetComponent<Renderer>().material = iceBlueBloom; // Player Bloom
                
                // Disable all SpotLight Lights (AllDark)
                for (int i = 0; i < SpotLights.Count; i++) {
                    SpotLights[i].gameObject.SetActive(false);
                }

                // Debug.Log("LEVEL 05: MaxDrag = " + maxDrag + " / MaxKillerToSpwn = " + maxStonesToSpawn + " - #GameManager.cs - Update()");                 
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