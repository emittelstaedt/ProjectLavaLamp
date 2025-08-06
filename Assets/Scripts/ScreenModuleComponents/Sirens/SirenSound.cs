using UnityEngine;

public class SirenSound : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    private GameObject soundGameObject;

    public void StartSirenSound()
    {
        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.SFX, SoundType.Alarm, volume, transform.position);
    }

    public void StopSirenSound()
    {
        if (soundGameObject != null)
        {
            soundGameObject.SetActive(false);
        }
    }
}