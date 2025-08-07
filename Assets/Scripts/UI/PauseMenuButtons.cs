using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private GameObject confirmMainMenuPanel;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private string sceneName = "OfficeWorkplace";

    public void UnpauseGame()
    {
        pauseMenuManager.WasUnpaused = true;
        currentMenu.SetActive(false);

        // Stops the selection animation of the resume button.
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void LoadMainMenu()
    {
        confirmMainMenuPanel.SetActive(true);
    }

    public void ConfirmMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
        UnpauseGame();

        loadingScreen.SetActive(true);

        mainMenu.SetActive(true);

        SceneLoader.Instance.UnloadScene(sceneName);
    }

    public void CancelMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
    }
}
