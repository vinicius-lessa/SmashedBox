using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 19/07/2021

    Descrição:
        Muda a cor do Texto de um objeto do tipo texto quando passado o mouse em cima do mesmo.
        Utilizado atualmente para:
            - Botão de "BACK" ao estar visualizando a tabela de HighScore

*/

 public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
 {
    // private bool mouse_over = false;

    private void OnEnable() {
        // Cor DEFAULT do texto
        // gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    private void Update() {
        // if (mouse_over) {
        //     // Debug.Log("Mouse Over");
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // mouse_over = true;
       // // Debug.Log("Mouse exit");
       gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
       // mouse_over = false;        
       // // Debug.Log("Mouse exit");
       gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;        
    }
 }

