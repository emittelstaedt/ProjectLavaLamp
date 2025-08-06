using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private MixerType mixerType;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat(mixerType.ToString() + "Volume", slider.value);
        slider.value = savedVolume;

        UpdateText(slider.value);

        OnSliderChanged(savedVolume);
    }

    public void OnSliderChanged(float value)
    {
        AudioManager.Instance.SetMixerVolume(mixerType, value);

        PlayerPrefs.SetFloat(mixerType.ToString() + "Volume", value);
        PlayerPrefs.Save();

        UpdateText(value);
    }
    
    private void UpdateText(float value) 
    {
        if (valueText != null)
        {
            float displayValue = Mathf.Lerp(0f, 100f, value);
            valueText.text = displayValue.ToString("F0");
        }
    }
}