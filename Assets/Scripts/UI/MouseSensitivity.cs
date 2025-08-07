using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI valueText;

    void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.15f);
        CameraControls.Sensitivity = savedSensitivity;

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = CameraControls.Sensitivity * 100f;
            UpdateSensitivity(sensitivitySlider.value);
        }
    }

    public void UpdateSensitivity(float value)
    {
        CameraControls.Sensitivity = value / 100f;
        PlayerPrefs.SetFloat("MouseSensitivity", CameraControls.Sensitivity);

        if (valueText != null)
        {
            float displayValue = Mathf.Lerp(50f, 150f, (value - 1f) / 29f);
            valueText.text = displayValue.ToString("F0");
        }
    }
}
