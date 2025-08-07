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
            soundTypeToClip.Add(soundClips[i].type, soundClips[i]);
        }
        
        audioPlayer = GetComponent<AudioPlayer>();
        mixerController = GetComponent<MixerController>();
    }

    private void Start()
    {
        foreach (MixerType type in System.Enum.GetValues(typeof(MixerType)))
        {
            float typeVolume = PlayerPrefs.GetFloat(type + "Volume", 0.15f);
            SetMixerVolume(type, typeVolume);
        }
    }

    public void SetMixerVolume(MixerType mixerType, float volume)
    {
        mixerController.SetVolume(mixerType, volume);
    }

    /// <summary>
    /// Plays sound once in 2D space.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        audioPlayer.PlaySound(mixerGroup, clip, volume);
    }

    /// <summary>
    /// Plays sound once at static 3D position.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume, Vector3 position)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        audioPlayer.PlaySound(mixerGroup, clip, volume, position);
    }

    /// <summary>
    /// Plays sound once at dynamic 3D position, following the parent.
    /// </summary>
    public void PlaySound(MixerType mixerType, SoundType soundType, float volume, Transform parent)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        audioPlayer.PlaySound(mixerGroup, clip, volume, parent);
    }

    /// <summary>
    /// Plays sound on loop in 2D space.
    /// Returns GameObject for the caller to disable to end the loop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume);
    }

    /// <summary>
    /// Plays sound on loop at static 3D position.
    /// Returns GameObject for the caller to disable to end the loop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Vector3 position)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, position);
    }

    /// <summary>
    /// Plays sound on loop at dynamic 3D position, following the parent.
    /// Returns GameObject for the caller to disable to end the loop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Transform parent)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        return audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, parent);
    }

    private (AudioMixerGroup, AudioClip) GetMixerAndClip(MixerType mixerType, SoundType soundType)
    {
        AudioMixerGroup mixerGroup = mixerController.GetMixerGroup(mixerType);
        AudioClip clip = GetAudioClip(soundType);

        return (mixerGroup, clip);
    }

    private AudioClip GetAudioClip(SoundType type)
    {
        return soundTypeToClip[type].GetAudioClip();
    }
}
