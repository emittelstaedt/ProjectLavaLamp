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
    
    private string sceneName = "MainGameOfficeBase";

    public void SetAction(string action)
    {
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
    }

    private void LoadMainMenu()
    {
        UnpauseGame();

        mainMenuCamera.SetActive(true);
        mainMenu.gameObject.SetActive(true);
        SetMainMenuAudioListener();

        SceneLoader.Instance.UnloadScene(sceneName);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator SetMainMenuAudioListener()
    {
        // Wait until main game is unloaded to avoid multiple audio listeners in the scene.
        yield return new WaitForEndOfFrame();
        mainMenuCamera.GetComponent<AudioListener>().enabled = true;
    }
}
