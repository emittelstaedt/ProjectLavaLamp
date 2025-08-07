using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject confirmQuitPanel;
    [SerializeField] private GameObject MainMenuCamera;
    private string sceneName = "MainGameOfficeBase";

    private void OnEnable()
    {
        Time.timeScale = 1f;
    }

    public void LoadOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(true);
    }

    public void LoadGame()
    {
        MainMenuCamera.GetComponent<AudioListener>().enabled = false;
        currentMenu.SetActive(false);

        SceneLoader.Instance.LoadScene(sceneName);
        MainMenuCamera.gameObject.SetActive(false);
        InputSystem.actions.FindActionMap("Player").Enable();
    }

    public void QuitGame()
    {
        confirmQuitPanel.gameObject.SetActive(true);
    }

    public void ConfirmQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void CancelQuit()
    {
        confirmQuitPanel.SetActive(false);
    }
}
