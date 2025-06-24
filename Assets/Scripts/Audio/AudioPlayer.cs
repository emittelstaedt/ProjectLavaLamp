using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playSoundPrefab;
    private ObjectPool audioPool;
    
    void Awake()
    {
        // Make an object pool.
        audioPool = new ObjectPool(playSoundPrefab, transform);
    }
    
    // Plays the given sound with no position one time.
    public void PlaySound(AudioMixerGroup mixerGroup, AudioClip clip, float volume)
    {
        // Get an audio source from the object pool.
        GameObject audioObject = audioPool.GetInstance();
        AudioSource source = audioObject.GetComponent<AudioSource>();

        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = 0;
        
        // Play sound, then deactivate the object after the sound is done playing.
        source.PlayOneShot(clip, volume);
        StartCoroutine(DeactivateAfterSeconds(audioObject, clip.length));
    }
    
    // Plays the given sound with no position on loop.
    // Returns the GameObject so that the caller can disable the object when they want the loop to stop.
    public GameObject PlaySoundLoop(AudioMixerGroup mixerGroup, AudioClip clip, float volume)
    {
        // Get an audio source from the object pool.
        GameObject audioObject = audioPool.GetInstance();
        AudioSource source = audioObject.GetComponent<AudioSource>();
        
        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = 0;
        
        // Set and start playing the clip
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();
        return audioObject;
    }
    
    // Plays the given sound at the given position once.
    public void PlaySound(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Vector3 position)
    {
        // Get an audio source from the object pool.
        GameObject audioObject = audioPool.GetInstance();
        AudioSource source = audioObject.GetComponent<AudioSource>();
        
        audioObject.transform.position = position;
        
        source.outputAudioMixerGroup = mixerGroup;
        
        // Enables 3d sound.
        source.spatialBlend = 1;

        // Play the clip, disable the object when done.
        source.PlayOneShot(clip, volume);
        StartCoroutine(DeactivateAfterSeconds(audioObject, clip.length));
    }
    
    // Plays the given sound at the given position on loop.
    // Returns the GameObject so that the caller can disable the object when they want the loop to stop.
    public GameObject PlaySoundLoop(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Vector3 position)
    {
        // Get an audio source from the object pool.
        GameObject audioObject = audioPool.GetInstance();
        AudioSource source = audioObject.GetComponent<AudioSource>();
        
        audioObject.transform.position = position;

        source.outputAudioMixerGroup = mixerGroup;
        
        // Enables 3d sound.
        source.spatialBlend = 1;
        
        // Play until the caller disables the object.
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();
        return audioObject;
    }

    // Deactivates the given game object after the given seconds.
    private IEnumerator DeactivateAfterSeconds(GameObject audioObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audioObject.SetActive(false);
    }
}
