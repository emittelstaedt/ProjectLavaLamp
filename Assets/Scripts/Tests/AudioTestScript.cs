using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioTestScript : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    private float time = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SoundTest());
    }

    void Update()
    {
        time += Time.deltaTime;
    
        MoveX();
    }

    private void MoveX()
    {
        float newX = transform.position.x + speed * Time.deltaTime;
        
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void MoveInCircle()
    {
        float circleRadius = transform.position.magnitude;
        
        float newX = Mathf.Cos(speed * time) * circleRadius;
        float newY = Mathf.Sin(speed * time) * circleRadius;
        
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    // Test all the different overloads for the audio manager
    private IEnumerator SoundTest()
    {
        GameObject audioObject = AudioManager.Instance.PlaySoundLoop(MixerType.SFX, SoundType.Unknown, 1f, transform);
        yield return new WaitForSeconds(5);
        audioObject.SetActive(false);
        transform.position = Vector3.zero;
        
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Unknown, 1f, transform);
        yield return new WaitForSeconds(10);
        
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Unknown, 1f);
        yield return new WaitForSeconds(3);

        AudioManager.Instance.SetVolume(MixerType.SFX, 0.2f);        
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Unknown, 1f);
        yield return new WaitForSeconds(5);
        
        AudioManager.Instance.PlaySound(MixerType.Music, SoundType.Unknown, 1f, -transform.position);
        yield return new WaitForSeconds(5);

        audioObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.Unknown, 0.5f);
        yield return new WaitForSeconds(5);
        audioObject.SetActive(false);
        
        audioObject = AudioManager.Instance.PlaySoundLoop(MixerType.UI, SoundType.Unknown, 1f, transform.position);
        yield return new WaitForSeconds(5);
        audioObject.SetActive(false);
        
        SceneManager.LoadScene("AudioManager");
    }
}
