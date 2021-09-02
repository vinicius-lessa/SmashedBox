using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 19/07/2021

    Descrição:
        Player é a classe criada para o objeto "jogável" do nosso Game, poderia ter qualquer outro nome
        Ela herda tudo da classe MonoBehaviour

    Links:
        Basics: 
            Controls: https://youtu.be/p-3S73MaDP8

        LeanTween:
            https://assetstore.unity.com/packages/tools/animation/leantween-3595

        Formatos de Movimento (Boost - setEase...):
            https://codepen.io/jhnsnc/pen/LpVXGM
*/

public class Player : MonoBehaviour
{
    // # Gamepad Controls
    PlayerControls controls;

    // # Player Movement Properties
    private float forceMultiplier = 9;
    private float maximumVelocity = 3.8f;

    // # Jumping
    private float jumpForce = 1.3f;
    bool cubeIsontheGround = true;
    private bool firstCollisionOcured = false;    
    private float pongForce = 1.5f; // Invisible Wall Pong Player    
    
    // # BoxWalk SFX
    private AudioSource asFootStepOne, asFootStepTwo, asFootStepThree;      // (AudioSystem)
    private List<AudioSource> listAudioSource = new List<AudioSource>();
    private int previousSort;   

    // # Exmplosion
    public GameObject plankWood;
    public float cubeSize = 0.2f;
    public float cubeMass = 0.2f;
    public int cubesInRow = 5;
    public float explosionForce = 40;
    public float explosionRadius = 4;
    public float explosionUpward = .4f;
    float cubesPivotDistance;
    Vector3 cubesPivot;
    
    // PlayerFootDust
    public ParticleSystem footDust;
    private float distanceDust = .1f;

    // Rock Destruction
    public ParticleSystem rockDust;

    // Boost  
    private float timer;    
    private float boostReloadTime = 5f;
    private float boostPower = 3.5f;
    private bool boostIsAvailible = true;
    public Slider boostSlider; // Impulse Slider
    

    // Player Components
    private Rigidbody rb;

    // # Camera Zoom
    private bool isProcessingZoom;

    void Awake()
    {
        asFootStepOne   = GameObject.Find("[AS] BoxFootStepOne").gameObject.GetComponent<AudioSource>();
        asFootStepTwo   = GameObject.Find("[AS] BoxFootStepTwo").gameObject.GetComponent<AudioSource>();
        asFootStepThree = GameObject.Find("[AS] BoxFootStepThree").gameObject.GetComponent<AudioSource>();
        listAudioSource.Add(asFootStepOne);
        listAudioSource.Add(asFootStepTwo);
        listAudioSource.Add(asFootStepThree);

        controls = new PlayerControls();
        boostSlider.value = boostSlider.maxValue;

        // Jump (GAMEPAD)
        controls.Gameplay.Jump.performed += ctx => JumpPlayer();

        // Boost (GAMEPAD)
        controls.Gameplay.Boost.performed += ctx => BoostPlayer();

        // Gamepad (Move)
        // controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        // controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;

        // # Wooden Box EXPLOSION
        cubesPivotDistance = cubeSize * cubesInRow / 2; // Calcula a distância do Pivot        
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance); // Use este valor para criar o vetor do pivot
        
        // Fazendo Cashing do Método GET COMPONENT para o processamento ficar mais leve a cada frame
        rb = GetComponent<Rigidbody>();                  
    }

    void JumpPlayer(){
        var horizontalInput = Input.GetAxis("Horizontal");  // -1 > 1
        
        if (cubeIsontheGround){
            // FootStep DUST
            float direction = rb.GetPointVelocity(transform.position).x;                
            if (!(direction == 0)){
                float postitionX = direction < 0 ? gameObject.transform.position.x - distanceDust : gameObject.transform.position.x + distanceDust; // Ajusta posição X
                float postitionY = gameObject.transform.position.y - .5f; // Ajusta Altura
                float postionZ = gameObject.transform.position.z; // Default
                
                Vector3 pos = new Vector3(postitionX, postitionY, postionZ);
                Vector3 rot = new Vector3(footDust.transform.position.x, direction < 0 ? 90f : -90f ,footDust.transform.position.z);

                footDust.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                footDust.Play();
            }

            FindObjectOfType<AudioManager>().Play("[FX] Jump");
            // Vector3.up significa EIXO Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);            
            cubeIsontheGround = false;
        }            
    }

    void BoostPlayer(){                
        var horizontalInput = Input.GetAxis("Horizontal");
        
        if (boostIsAvailible) {
            applyBoost(horizontalInput);
            boostIsAvailible = false;
        }
    }

    void OnEnable() {
        controls.Gameplay.Enable();
    }

    void OnDisable() {
        controls.Gameplay.Disable();
    }    

    void Update(){
        var horizontalInput = Input.GetAxis("Horizontal");  // -1 > 1 // Keyboard or Gamepad
        
        if (rb.velocity.magnitude <= maximumVelocity && !PauseMenu.GameIsPause){ // MOVE                        
            rb.AddForce(new Vector3(horizontalInput * forceMultiplier, 0, 0));            
        }
        
        if (Input.GetKeyDown(KeyCode.Space) & cubeIsontheGround){   // JUMP
            // FootDust
            float direction = rb.GetPointVelocity(transform.position).x;                
            if (!(direction == 0)){
                float postitionX = direction < 0 ? gameObject.transform.position.x - distanceDust : gameObject.transform.position.x + distanceDust; // Ajusta posição X
                float postitionY = gameObject.transform.position.y - .5f; // Ajusta Altura
                float postionZ = gameObject.transform.position.z; // Default
                
                Vector3 pos = new Vector3(postitionX, postitionY, postionZ);
                Vector3 rot = new Vector3(footDust.transform.position.x, direction < 0 ? 90f : -90f ,footDust.transform.position.z);

                footDust.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                footDust.Play();
            }
            
            FindObjectOfType<AudioManager>().Play("[FX] Jump");
            // Vector3.up significa EIXO Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            cubeIsontheGround = false;
        }

        if (Input.GetMouseButtonDown(1)){   // BOOST
            //if (!(horizontalInput == 0) && boostIsAvailible) {
            if (boostIsAvailible) {
                applyBoost(horizontalInput);
                boostIsAvailible = false;
            }
        }
        
        if (!boostIsAvailible) {
            timer += Time.deltaTime;
            
            if (timer >= boostReloadTime){
                boostIsAvailible = true;
                timer = 0;
                // Debug.Log("Boost Disponível!");
            } else {
                if (!(timer > 5))
                    boostSlider.value = timer;
            }
        }

        // Reposition the player on the Z Axis
        if (transform.position.z != -4.90f){
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.90f);
        }
    }

    public void applyBoost (float horizontalInput) {
        FindObjectOfType<AudioManager>().Play("[FX] BoostSound");    
        
        boostSlider.value = 0; // Zero Impulse            
        
        if (horizontalInput < 0) { // Esquerda
            if (cubeIsontheGround)
                rb.AddForce(Vector3.up * (boostPower * .2f), ForceMode.Impulse);
                
            rb.AddForce(Vector3.left * boostPower, ForceMode.Impulse);        
        }            
        else if (horizontalInput > 0) { // Direita
            if (cubeIsontheGround)
                rb.AddForce(Vector3.up * (boostPower * .2f), ForceMode.Impulse);

            rb.AddForce(Vector3.right * boostPower, ForceMode.Impulse);
        } else if (horizontalInput == 0) { // Pra Cima
            rb.AddForce(Vector3.up * boostPower, ForceMode.Impulse);
        }

        StartCoroutine(FreezeRotation());
    }

    IEnumerator FreezeRotation() {
        // Turns Boost Bar to 0
        float currentTime   = 0;
        float targetValue   = 0;
        float duration      = 0.3f;
        float start         = boostSlider.maxValue;
        
        // Freeze the Rotation on Z Axis
        gameObject.transform.Find("[PS] Impulse").gameObject.GetComponent<ParticleSystem>().Play();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;        

        // Turns Boost Bar to 0
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            boostSlider.value = Mathf.Lerp(start, targetValue, currentTime / duration);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;        
        gameObject.transform.Find("[PS] Impulse").gameObject.GetComponent<ParticleSystem>().Stop();
    }

    void OnCollisionEnter(Collision collision) {
        cubeIsontheGround = collision.gameObject.CompareTag("invisibleWallTwo") ? false   : true  ; // Se tocar em Killer ou Floor

        // Igonres RockPieces
        if (collision.gameObject.CompareTag("RockPieces")){
            // Debug.Log("Ignorando");
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }

        // Impulse Destruction - Aditional Force when destructs a Rock
        if (collision.gameObject.CompareTag("RockOnFloor")) {
            if (!boostIsAvailible){                
                if (collision.relativeVelocity.magnitude > 8){                    
                    var horizontalInput = Input.GetAxis("Horizontal");  // -1 > 1 // Keyboard or Gamepad 
                    if (horizontalInput > 0)
                        rb.AddForce(Vector3.right * 4, ForceMode.Impulse);
                    else if (horizontalInput < 0)
                        rb.AddForce(Vector3.left * 4, ForceMode.Impulse);
                    
                    rockDust.Play(); // ParticleSystem
                    
                    int randomCamera = Random.Range(0, 2);

                    if (randomCamera == 0 && !isProcessingZoom) {
                        FindObjectOfType<MainCamareManager>().MoveCameraToPlayer();
                        isProcessingZoom = true;
                        StartCoroutine(AwaitForZoom());
                    }                        
                }                    
            }
        }        

        // # Invisble Wall
        if (collision.gameObject.CompareTag("invisibleWallTwo")) {            
            var horizontalInput = Input.GetAxis("Horizontal");  // -1 > 1 // Keyboard or Gamepad 
            float horizontalForcePong = pongForce * 0.333f;

            FindObjectOfType<AudioManager>().Play("[FX] Pong");
            
            // "Pong" wall
            rb.AddForce(Vector3.down * pongForce, ForceMode.Impulse);
                                    
            if (collision.relativeVelocity.magnitude < 8) { // Evitar bug de velocidade
                if (horizontalInput > 0) //  Right Touch
                    rb.AddForce(Vector3.left * (pongForce + horizontalForcePong), ForceMode.Impulse);
                else if (horizontalInput < 0) // Left Touch
                    rb.AddForce(Vector3.right * (pongForce + horizontalForcePong), ForceMode.Impulse);
            }
        }

        // # GameOver
        if (collision.gameObject.CompareTag("Killer")){
            if (collision.transform.position.y >= (transform.position.y+0.8f)){ // Tentativa de corrigir bug de quebrar caixa mesmo batendo de lado quando a pedra está no ar                
                GameManager.GameOver();
                explode();
            }            
        }
        
        // # Floor
        if (collision.gameObject.CompareTag("Floor")){
            // FootStep SFX
            if (firstCollisionOcured){                
                int sortBornFX;
                sortBornFX = Random.Range(0, 3);
                if (previousSort == sortBornFX){
                    if (previousSort == 2){
                        listAudioSource[sortBornFX--].Play();
                    } else {
                        listAudioSource[sortBornFX++].Play();
                    }
                } else {
                    listAudioSource[sortBornFX].Play();
                }
                previousSort = sortBornFX;
            } else {
                firstCollisionOcured = true;
            }            
                    
            // Player Walking DUST
            float direction = ( rb.GetPointVelocity(transform.position).x == 0 ? 0 : rb.GetPointVelocity(transform.position).x );
            if (!(direction == 0)){                
                ContactPoint contact = collision.contacts[0];
                Vector3 pos = contact.point;
                // Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);                        
                footDust.transform.SetPositionAndRotation(pos, Quaternion.Euler(new Vector3(footDust.transform.position.x, direction < 0 ? 90f : -90f ,footDust.transform.position.z)));                    
                footDust.Play();
            }   
        }               
    }

    private IEnumerator AwaitForZoom () {
        yield return new WaitForSeconds(3f);
        isProcessingZoom = false;
    }

    public void explode () 
    {
        gameObject.SetActive(false);
        
        // Loop 3 vezes para criar 5x5x5 de pedaços nas coordenadas X, Y, Z
        for (int x = 0; x < cubesInRow; x++){
            for (int y = 0; y < cubesInRow; y++){
                for (int z = 0; z < cubesInRow; z++){
                    createPiece(x, y, z);
                }
            }                
        }

        // Retorna posição da Explosão
        Vector3 explosionPos = transform.position;
        // Retorna colliders nessa posição e radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        // Adiciona força de Explosão a todos os colliders nesse Overlap Sphere
        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }
    }

    void createPiece (int x, int y, int z)
    {
        // Instancia um novo objeto
        GameObject piece;
        piece = GameObject.Instantiate(plankWood);
               
        // Seta Escala
        plankWood.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        plankWood.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        // Seta a MASSA        
        plankWood.GetComponent<Rigidbody>().mass = cubeMass;        
    }
}