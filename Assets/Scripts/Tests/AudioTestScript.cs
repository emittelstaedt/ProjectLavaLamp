using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioTestScript : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SoundTest());
    }

    private IEnumerator SoundTest()
    {
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Unknown, 1f);
        yield return new WaitForSeconds(clip.length / 2);

        AudioManager.Instance.SetVolume(MixerType.SFX, 0.2f);        
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Unknown, 1f);
        yield return new WaitForSeconds(clip.length);
        
        AudioManager.Instance.PlaySound(MixerType.Music, SoundType.Unknown, 1f, -transform.position);
        yield return new WaitForSeconds(clip.length);

        GameObject audioObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.Unknown, 0.5f);
        yield return new WaitForSeconds(clip.length * 1.5f);
        audioObject.SetActive(false);
        
        audioObject = AudioManager.Instance.PlaySoundLoop(MixerType.UI, SoundType.Unknown, 1f, transform.position);
        yield return new WaitForSeconds(clip.length * 1.5f);
        audioObject.SetActive(false);
        SceneManager.LoadScene("AudioManager");
    }
}
