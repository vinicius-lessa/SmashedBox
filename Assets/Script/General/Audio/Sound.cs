using UnityEngine;

/*
### - DOC

    Criador(es): VINÍCIUS LESSA (LessLax Studios)

    Data: 08/07/2021

    Descrição:
               
    Definições:
        
*/

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float spatialBlend;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
