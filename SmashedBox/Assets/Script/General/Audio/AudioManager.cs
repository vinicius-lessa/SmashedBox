using System;
using System.Collections;
using UnityEngine;

/*
 * @Documentaion
 * 
 * DESCRIPTION
 *      Class responsible for managing all audio features in the game
 *      
 * DATES
 *      08/07/2021 - Vinícius Lessa (LessLax): Creation of script
 *      10/03/2024 - Vinícius Lessa (LessLax): Inspection of the code and documentation updating
 *   
*/

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake() {
        
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source                = gameObject.AddComponent<AudioSource>();
            s.source.clip           = s.clip;             
            s.source.volume         = s.volume;
            s.source.pitch          = s.pitch;
            s.source.spatialBlend   = s.spatialBlend;
            s.source.loop           = s.loop;            
        }
    }

    void Start() {
        // Caso haja uma música para toda a cena
        // Play("Theme");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);       

        if (s == null)
        {
            // Debug.LogWarning("Sound: " + name + " not found!");
        }
        
        s.source.Play();
    }

    public bool IsPlaying (string name)
    {
        bool isPlaying;
        Sound s = Array.Find(sounds, sound => sound.name == name);       

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        
        return isPlaying = s.source.isPlaying;
    }


    public void FadeAudio (string name, float duration, bool isFadeInOrOut)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);       

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        
        if (isFadeInOrOut)  // Fade In: true, Fade Out: false
            s.source.Play();

        //                                       AudioSource audioSource, float duration, float targetVolume, bool isFadeInOrOut
        StartCoroutine(FadeAudioSource.StartFade(s.source               , duration      , s.volume          , isFadeInOrOut));
    }

    public void Mute (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);       

        if (s == null)
        {
            // Debug.LogWarning("Sound: " + name + " not found!");
        }

        s.source.mute = true;        
    }

    public void Unmute (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);       

        if (s == null)
        {
            // Debug.LogWarning("Sound: " + name + " not found!");
        }

        s.source.mute = false;        
    }

    public void Stop (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            // Debug.LogWarning("Sound: " + name + " not found!");
        }

        s.source.Stop();
    }
}