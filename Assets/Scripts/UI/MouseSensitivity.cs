using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI valueText;

    void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity * 100f;
            UpdateSensitivity(sensitivitySlider.value);
        }
    }

    public void UpdateSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value / 100f);

        if (valueText != null)
        {
            float displayValue = Mathf.Lerp(50f, 150f, (value - 1f) / 29f);
            valueText.text = displayValue.ToString("F0");
        }
    }
}
