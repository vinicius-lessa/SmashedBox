using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FadeAudioSource {

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, bool isFadeInOrOut)
    {
        // ### FADE IN - Fade In: true, Fade Out: false
        if (isFadeInOrOut) {
            float currentTime = 0;
            float start = 0;        

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        } 
        // ### FADE OUT - Fade In: true, Fade Out: false
        else {
            float currentTime = 0;
            float start = audioSource.volume;   
            targetVolume = 0; // Substitute the original value     

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }

            audioSource.Stop(); // End the Sound
            yield break;
        }
    }
}