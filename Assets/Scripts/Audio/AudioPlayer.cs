using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playSoundPrefab;
    private ObjectPool audioPool;
    
    void Awake()
    {
        audioPool = new ObjectPool(playSoundPrefab, transform);
    }

    // Plays sound once in 2D space.
    public void PlaySound(AudioMixerGroup mixerGroup, AudioClip clip, float volume)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 0f);

        source.PlayOneShot(clip, volume);
        StartCoroutine(DeactivateAfterSeconds(audioObject, clip.length));
    }

    // Plays sound once at static 3D position.
    public void PlaySound(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Vector3 position)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 1f);

        audioObject.transform.position = position;

        source.PlayOneShot(clip, volume);
        StartCoroutine(DeactivateAfterSeconds(audioObject, clip.length));
    }

    // Plays sound once at dynamic 3D position, following the parent.
    public void PlaySound(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Transform parent)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 1f);

        audioObject.transform.SetParent(parent, false);

        source.PlayOneShot(clip, volume);
        StartCoroutine(DeactivateAfterSeconds(audioObject, clip.length));
    }

    // Plays sound on loop in 2D space.
    public GameObject PlaySoundLoop(AudioMixerGroup mixerGroup, AudioClip clip, float volume)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 0f);

        PlayLooped(source, clip, volume);
        return audioObject;
    }

    // Plays sound on loop at static 3D position.
    public GameObject PlaySoundLoop(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Vector3 position)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 1f);

        audioObject.transform.position = position;
        
        PlayLooped(source, clip, volume);
        return audioObject;
    }

    // Plays sound on loop at dynamic 3D position, following the parent.
    public GameObject PlaySoundLoop(AudioMixerGroup mixerGroup, AudioClip clip, float volume, Transform parent)
    {
        var (audioObject, source) = InitializeAudioSourceObject(mixerGroup, 1f);

        audioObject.transform.SetParent(parent, false);

        PlayLooped(source, clip, volume);
        return audioObject;
    }

    private (GameObject, AudioSource) InitializeAudioSourceObject(AudioMixerGroup mixerGroup, float spatialBlend)
    {
        GameObject audioObject = audioPool.GetInstance();
        AudioSource source = audioObject.GetComponent<AudioSource>();

        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = spatialBlend;

        return (audioObject, source);
    }

    private void PlayLooped(AudioSource source, AudioClip clip, float volume)
    {
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();
    }

    // Return the audio object to the pool after given time.
    private IEnumerator DeactivateAfterSeconds(GameObject audioObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audioObject.SetActive(false);
    }
}