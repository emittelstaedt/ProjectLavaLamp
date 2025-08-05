using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float fillDuration = 2f;
    private float elapsedTime;

    private void OnEnable()
    {
        loadingSlider.value = 0;
        elapsedTime = 0f;
        StartCoroutine(Fill());
    }

    private IEnumerator Fill()
    {
        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float step = Mathf.Clamp01(elapsedTime / fillDuration);
            loadingSlider.value = Mathf.SmoothStep(0f, 1f, step);

            yield return null;
        }
        loadingSlider.value = 1f;
        transform.parent.gameObject.SetActive(false);
    }
}
