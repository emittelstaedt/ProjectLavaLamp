using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundClip[] soundClips;
    private AudioPlayer audioPlayer;
    private MixerController mixerController;
    private Dictionary<SoundType, SoundClip> soundTypeToClip;

    public static AudioManager Instance = null;

    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<AudioManager>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
        // Initialize dictionary to map SoundTypes to SoundClips.
        soundTypeToClip = new Dictionary<SoundType, SoundClip>();
        for (int i = 0; i < soundClips.Length; i++)
        {
            soundTypeToClip.Add(soundClips[i].Type, soundClips[i]);
        }
        
        audioPlayer = GetComponent<AudioPlayer>();
        mixerController = GetComponent<MixerController>();
    }
    
    public void SetMixerVolume(MixerType mixerType, float volume)
    {
        mixerController.SetVolume(mixerType, volume);
    }
    
    /// <summary>
    /// Plays the given sound with no position one time.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        audioPlayer.PlaySound(mixerGroup, clip, volume);
    }

    /// <summary>
    /// Plays the given sound with no position and loops.
    /// Returns the GameObject so that the caller can disable the object when they want the loop to stop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume);
    }
    
    /// <summary>
    /// Plays the given sound at the given position one time.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume, Vector3 position)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        audioPlayer.PlaySound(mixerGroup, clip, volume, position);
    }
    
    /// <summary>
    /// Plays the given sound at the given position and loops.
    /// Returns the GameObject so that the caller can disable the object when they want the loop to stop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Vector3 position)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, position);
    }
    
    /// <summary>
    /// Plays the given sound while following the given transform once.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume, Transform parent)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        audioPlayer.PlaySound(mixerGroup, clip, volume, parent);
    }
    
    /// <summary>
    /// Plays the given sound as a child of the given transform on loop.
    /// Returns the GameObject so that the caller can disable the object when they want the loop to stop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Transform parent)
    {
        AudioClip clip = GetAudioClip(soundType);
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        
        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, parent);
    }
    
    private AudioClip GetAudioClip(SoundType type)
    {
        return soundTypeToClip[type].GetAudioClip();
    }
}
