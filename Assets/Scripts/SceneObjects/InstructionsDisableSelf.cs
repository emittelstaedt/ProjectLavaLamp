using UnityEngine;
using System.Collections;

public class InstructionsDisableSelf : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pull the data out of the Level Manager
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            if(LevelManager.Instance.currentSession.currentDay==1) //If we are on day 1
            {
                if(transform.position.z<=-4) //If we are on day one AND we are outside the elevator
                {
                    StartCoroutine(FadeOutRoutine()); //Fade
                }
            }
            else //We are NOT on day one
            {
                gameObject.SetActive(false); //Just disable self
            }  
        }
        else
        {
            Debug.LogWarning("Instructions could not get day from levelmanager!");
            gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeOutRoutine(){
        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null; // Wait for the next frame
        }

        spriteRenderer.color = targetColor; // Ensure it ends at 0
    }


}
