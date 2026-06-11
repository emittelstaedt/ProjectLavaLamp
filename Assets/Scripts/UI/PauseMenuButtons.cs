using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

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
        loadingScreen.SetActive(true);
		StartCoroutine(WaitToUnloadScene());
		
    }

	private IEnumerator WaitToUnloadScene(){
		SceneLoader.Instance.UnloadScene(sceneName);
		yield return null;
		UnpauseGame();
		mainMenu.SetActive(true);
	}
	
    public void CancelMainMenuLoad()
    {
        confirmMainMenuPanel.SetActive(false);
    }
}
