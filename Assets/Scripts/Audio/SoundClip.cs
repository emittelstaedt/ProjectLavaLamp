using UnityEngine;

[System.Serializable]
public struct SoundClip
{
    [SerializeField] public AudioClip[] clips;
    [SerializeField] public SoundType type;

    public AudioClip GetAudioClip()
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning("No audio clips available.");
            return null;
        }
        else if (clips.Length > 1)
        {
            // Swap random non-first clip with the first one to prevent clip from playing twice in a row
            int index = Random.Range(1, clips.Length);

            AudioClip tempClip = clips[index];
            clips[index] = clips[0];
            clips[0] = tempClip;
        }

        return clips[0];
    }
}
