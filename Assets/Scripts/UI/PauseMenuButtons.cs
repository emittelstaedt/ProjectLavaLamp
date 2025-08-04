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
    
    private string sceneName = "MainGameOfficeBase";

    public void SetAction(string action)
    {
        Time.timeScale = 1f;
        Invoke(action, 0.6f);
    }

    private void UnpauseGame()
    {
        pauseMenuManager.WasUnpaused = true;
        currentMenu.SetActive(false);

        // Stops the selection animation of the resume button.
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void LoadOptionsMenu()
    {
        OptionsMenuButtons.LastMenu = currentMenu;
        currentMenu.SetActive(false);
        optionsMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    private void LoadMainMenu()
    {
        confirmMainMenuPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ConfirmMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
        UnpauseGame();

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
