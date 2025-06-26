using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetVolume(MixerType mixerType, float volume)
    {
        audioMixer.SetFloat(mixerType.ToString() + "Volume", AmplitudeToDB(volume));
    }
    
    public AudioMixerGroup GetMixerGroup(MixerType type)
    {
        return audioMixer.FindMatchingGroups(type.ToString())[0];
    }
    
    private float AmplitudeToDB(float amplitude)
    {
        return Mathf.Log10(amplitude) * 20;
    }
}
