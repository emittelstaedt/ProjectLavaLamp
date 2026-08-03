using UnityEngine;

public class SirenSound : MonoBehaviour
{
	[SerializeField] private Animator sirenAnimator;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    private GameObject soundGameObject;

    public void StartSirenSound()
    {
        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.SFX, SoundType.Alarm, volume, transform.position);
		sirenAnimator.SetBool("engageSiren", true);
    }

    public void StopSirenSound()
    {
        if (soundGameObject != null)
        {
            soundGameObject.SetActive(false);
			sirenAnimator.SetBool("engageSiren", false);
        }
    }

    void OnDisable()
    {
        if (soundGameObject != null)
        {
            soundGameObject.SetActive(false);
			sirenAnimator.SetBool("engageSiren", false);
        }
    }
}