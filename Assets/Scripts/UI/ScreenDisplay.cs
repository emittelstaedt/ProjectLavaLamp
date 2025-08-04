using UnityEngine;
using System.Collections.Generic;

public class ScreenDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown qualityDropdown;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMPro.TMP_Dropdown fullscreenDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        int qualityIndex = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        qualityDropdown.value = qualityIndex;
        qualityDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(qualityIndex);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);

        int currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Resolution resolution = resolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        int fullScreenIndex = PlayerPrefs.GetInt("FullscreenIndex", 1);
        fullscreenDropdown.value = fullScreenIndex;
        fullscreenDropdown.RefreshShownValue();

        if (fullScreenIndex == 0)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(int fullScreenIndex)
    {
        if(fullScreenIndex == 0)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        PlayerPrefs.SetInt("FullScreenIndex", fullScreenIndex);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }
}
