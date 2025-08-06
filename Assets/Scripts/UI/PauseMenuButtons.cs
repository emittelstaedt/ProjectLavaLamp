using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuCamera;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private GameObject confirmMainMenuPanel;
    [SerializeField] private GameObject loadingScreen;
    
    private string sceneName = "MainGameOfficeBase";

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseMenuManager.WasUnpaused = true;
        currentMenu.SetActive(false);

        // Stops the selection animation of the resume button.
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadOptionsMenu()
    {
        Time.timeScale = 1f;
        OptionsMenuButtons.LastMenu = currentMenu;
        currentMenu.SetActive(false);
        optionsMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        confirmMainMenuPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ConfirmMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
        UnpauseGame();

        loadingScreen.SetActive(true);

        mainMenuCamera.SetActive(true);
        mainMenu.gameObject.SetActive(true);
        SetMainMenuAudioListener();

        SceneLoader.Instance.UnloadScene(sceneName);
    }

    public void CancelMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
    }

    private IEnumerator SetMainMenuAudioListener()
    {
        // Wait until main game is unloaded to avoid multiple audio listeners in the scene.
        yield return new WaitForEndOfFrame();
        mainMenuCamera.GetComponent<AudioListener>().enabled = true;
    }
}
