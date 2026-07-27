using UnityEngine;
using System.Collections;

public class SetGlobalCorrosion : MonoBehaviour
{
    [SerializeField] private float corrosionTier1 = 0.3333f;
    [SerializeField] private float corrosionTier2 = 0.6666f;
    [SerializeField] private float corrosionTier3 = 1f;
    [SerializeField] private float startingCorrosion = 0f;
	[SerializeField] private float startingDisappear = 0f;
    [SerializeField] private float serializedDuration = 3f;
	[SerializeField] private VoidEventChannelSO triggerDisappear;
	
    private static readonly int CorrosionID = Shader.PropertyToID("_Damage_Amount");
	private static readonly int DisappearID = Shader.PropertyToID("_Fade_amount");
    
	private void Awake()
    {
        if(LevelManager.Instance.currentSession!=null)
        {
            switch (LevelManager.Instance.currentSession.coffeeLevel)
            {
                case 0:
                    startingCorrosion = corrosionTier3;
                    break;
                case 1:
                    startingCorrosion = corrosionTier2;
                    break;
                case 2:
                    startingCorrosion = corrosionTier1;
                    break;
                default:
                    startingCorrosion = 0f;
                    break;
            }
            Shader.SetGlobalFloat(CorrosionID, startingCorrosion);
			Shader.SetGlobalFloat(DisappearID, startingDisappear);
        }
        else
        {
            Debug.LogWarning("Corrosion setter could not find level manager!");
        }
    }

    public void onCoffeeDrank()
    {
        StartCoroutine(FadeOutCorrosion(serializedDuration)); // Fade over serializeduration seconds
    }

    public IEnumerator FadeOutCorrosion(float duration)
    {
        float start = startingCorrosion;
		float startDis = startingDisappear;
        float elapsed = 0f;

		triggerDisappear.RaiseEvent();
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            Shader.SetGlobalFloat(CorrosionID, Mathf.Lerp(start, 0f, t));
			Shader.SetGlobalFloat(DisappearID, Mathf.Lerp(start, 1f, t));
            yield return null;
        }

        Shader.SetGlobalFloat(CorrosionID, 0f);
        startingCorrosion = 0f;
		startingDisappear = 1f;
    }
}
