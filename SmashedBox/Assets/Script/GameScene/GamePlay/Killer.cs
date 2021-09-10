using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 19/07/2021

    Descrição:
        Código atribuido ao Objeto que irá destruir o player. Este Script tem as seguintes funções:

            - Executar a "Destruição" do objeto "Killer" (ROCK) realizando diversas ações relacionadas a áudio, efeitos visuais, etc. (Explode()).
            - Define a Rotação que será aplicada ao objeto enquanto ele cai;
            
        Observações: Este código é chamado para cada DropBox que é SPAWNADO no Scrip GAMEMANAGER.
*/


public class Killer : MonoBehaviour
{
    // # Exmplosion
    public GameObject rockExplosion;
    private float   rockSize; // Random
    private float   rockMass          = 0.005f;
    private int     rocksInRow        = 3;
    private float   explosionForce    = 30f;
    private float   explosionRadius   = 10f;
    private float   explosionUpward   = 10f;
    
    float rocksPivotDistance;
    Vector3 rocksPivot;

    // Hit Text    
    private GameObject hitText;

    // # Air Rotation
    Vector3 rotation;
    private int randamRotation;
    
    // # Boleans
    private bool hitThePlayer = false; // if hit the Player Object
    private bool hitTheFloor = false; // if hit the Floor Object

    // # StoneFootDust
    public ParticleSystem stoneDust;

    // # Stone SFX
    public GameObject asStoneImpact;        // AudioSource
    public GameObject asStoneDestruction;   // AudioSource
    
    private void Awake() {
        rockExplosion = GameObject.Find("RockExplosion");        
    }

    private void Start(){
        // Way to instantiete a Prefab from another
        // hitText = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/UI/[TextHolder] HitStone.prefab");

        // Rock Destruction
        rocksPivotDistance = rockSize * rocksInRow / 2; // Calcula a distância do Pivot        
        rocksPivot = new Vector3(rocksPivotDistance, rocksPivotDistance, rocksPivotDistance); // Use este valor para criar o vetor do pivot

        // Dust PS
        stoneDust           = GameObject.Find("[PS] StoneImpact").GetComponent<ParticleSystem>();

        // AS SFX
        asStoneImpact       = GameObject.Find("[AS] StoneImpact"); // i'll use transform else, that's why i don't catch the audio source component directly (same for the bellow obj)
        asStoneDestruction  = GameObject.Find("[AS] StoneDestruction");

        // Rotation
        var xRotation = Random.Range(-2f, 2f);
        var yRotation = Random.Range(-2f, 2f);        
        if (yRotation == 0 ^ xRotation == 0) {
            rotation = new Vector3(1, 1);
        } else {            
            rotation = new Vector3(xRotation, yRotation);
        }
    }

    private void Update()
    {
        // Only will Rotate if is in the Air
        if (!hitThePlayer & !hitTheFloor)
            transform.Rotate(rotation);
    }

    private void OnCollisionEnter(Collision collision) 
    {
        // # Igonres Invisible Wall
        if (collision.gameObject.CompareTag("invisibleWall")){
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }
        
        // # ROCK DESTRUCTION
        if (transform.gameObject.tag == "RockOnFloor") {
            if (collision.gameObject.CompareTag("Player")){
                if (collision.relativeVelocity.magnitude > 8){
                    // Collect info about collision
                    ContactPoint contact    = collision.contacts[0];
                    Vector3 pos             = contact.point;
                    Quaternion rot          = Quaternion.FromToRotation(Vector3.up, contact.normal);
                    
                    // HitText (GameManager)
                    FindObjectOfType<GameManager>().DestroyedStone(this.transform);

                    explode();

                    // Impact SFX
                    asStoneDestruction.transform.SetPositionAndRotation(pos, rot);
                    asStoneDestruction.GetComponent<AudioSource>().Play();            
                }
            }
        }

        // # Floor
        if (collision.gameObject.CompareTag("Floor")){
            // Impact Dust        
            ContactPoint contact    = collision.contacts[0];
            Vector3 pos             = contact.point;
            Quaternion rot          = Quaternion.FromToRotation(Vector3.up, contact.normal);
            stoneDust.transform.SetPositionAndRotation(pos, rot);
            stoneDust.Play();
            
            // Impact SFX (with spatial blend)
            asStoneImpact.transform.SetPositionAndRotation(pos, rot);
            asStoneImpact.GetComponent<AudioSource>().Play();           

            transform.gameObject.tag = "RockOnFloor";
            hitTheFloor = true;
            Destroy(gameObject, 0.8f);
        }
        
        // Player
        if (collision.gameObject.CompareTag("Player")){
            hitThePlayer = true; // A partir daqui, as próximas colisões não serão válidas
        }                     
    }

    private void explode () {        
        gameObject.SetActive(false);
        
        // Loop 3 vezes para criar 5x5x5 de pedaços nas coordenadas X, Y, Z
        for (int x = 0; x < rocksInRow; x++){
            for (int y = 0; y < rocksInRow; y++){
                for (int z = 0; z < rocksInRow; z++){
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

    private void createPiece (int x, int y, int z) {
        // Instancia um novo objeto
        GameObject piece;        
        piece = GameObject.Instantiate(rockExplosion);
        rockSize = Random.Range(.1f,.05f);
               
        // Seta Escala
        piece.transform.position = transform.position + new Vector3(rockSize * x, rockSize * y, rockSize * z) - rocksPivot;
        piece.transform.localScale = new Vector3(rockSize, rockSize, rockSize);

        // Components
        piece.GetComponent<Rigidbody>().mass = rockMass;        

        // Destróis depois de segundos
        Destroy(piece, 2);        
    }
}