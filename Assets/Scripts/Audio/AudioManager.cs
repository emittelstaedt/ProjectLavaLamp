using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundClip[] soundClips;
    private AudioPlayer audioPlayer;
    private MixerController mixerController;
    private Dictionary<SoundType, SoundClip> soundTypeToClip;
    //For the queue sound type
    private Dictionary<AudioQueue, Queue<QueuedSound>> audioQueues;
    private Dictionary<AudioQueue, bool> queuePlaying;

    public static AudioManager Instance = null;

    //This is a helper class for specifically queued audio
    private class QueuedSound
    {
        public MixerType mixerType;
        public SoundType soundType;
        public float volume;

        public QueuedSound(MixerType mixerType, SoundType soundType, float volume)
        {
            this.mixerType = mixerType;
            this.soundType = soundType;
            this.volume = volume;
        }
    }

    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<AudioManager>();
        }
        else if (Instance != this)
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

        audioQueues = new Dictionary<AudioQueue, Queue<QueuedSound>>();
        queuePlaying = new Dictionary<AudioQueue, bool>();

        foreach (AudioQueue queue in System.Enum.GetValues(typeof(AudioQueue)))
        {
            audioQueues.Add(queue, new Queue<QueuedSound>());
            queuePlaying.Add(queue, false);
        }
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

        GameObject loopObject = audioPlayer.PlaySoundLoop(mixerGroup, clip, volume);
        TrackLoopedSound(loopObject);
        return loopObject;
    }

    /// <summary>
    /// Plays sound on loop at static 3D position.
    /// Returns GameObject for the caller to disable to end the loop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Vector3 position)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        GameObject loopObject = audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, position);
        TrackLoopedSound(loopObject);
        return loopObject;
    }

    /// <summary>
    /// Plays sound on loop at dynamic 3D position, following the parent.
    /// Returns GameObject for the caller to disable to end the loop.
    /// </summary>
    public GameObject PlaySoundLoop(MixerType mixerType, SoundType soundType, float volume, Transform parent)
    {
        var (mixerGroup, clip) = GetMixerAndClip(mixerType, soundType);

        GameObject loopObject = audioPlayer.PlaySoundLoop(mixerGroup, clip, volume, parent);
        TrackLoopedSound(loopObject);
        return loopObject;
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

    public void PlayQueuedSound(AudioQueue queue, MixerType mixerType, SoundType soundType, float volume)
    {
        audioQueues[queue].Enqueue(new QueuedSound(mixerType, soundType, volume));

        if (!queuePlaying[queue])
        {
            StartCoroutine(ProcessQueue(queue));
        }
    }

    private IEnumerator ProcessQueue(AudioQueue queue)
    {
        queuePlaying[queue] = true;

        while (audioQueues[queue].Count > 0)
        {
            QueuedSound sound = audioQueues[queue].Dequeue();

            var (mixerGroup, clip) = GetMixerAndClip(sound.mixerType, sound.soundType);

            audioPlayer.PlaySound(mixerGroup, clip, sound.volume);

            yield return new WaitForSeconds(clip.length);
        }

        queuePlaying[queue] = false;
    }

    //This makes sure all looped audio is stopped when officeworkplace is loaded.
    private const string OfficeSceneName = "OfficeWorkplace";

    private readonly List<GameObject> activeLoopedSounds = new List<GameObject>();

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += HandleSceneUnloaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= HandleSceneUnloaded;
    }

    private void TrackLoopedSound(GameObject loopObject)
    {
        if (loopObject == null) return;
        activeLoopedSounds.Add(loopObject);
    }

    private void HandleSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
        if (scene.name != OfficeSceneName) return;

        StopAllLoopedSounds();
    }

    /// <summary>
    /// Immediately stops (destroys) every looped sound currently tracked.
    /// </summary>
    public void StopAllLoopedSounds()
    {
        for (int i = 0; i < activeLoopedSounds.Count; i++)
        {
            GameObject loopObject = activeLoopedSounds[i];
            if (loopObject != null)
            {
                Destroy(loopObject);
            }
        }

        activeLoopedSounds.Clear();
    }

}
