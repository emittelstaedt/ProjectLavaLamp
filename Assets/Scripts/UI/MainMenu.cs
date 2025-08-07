using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject confirmQuitPanel;
    [SerializeField] private GameObject HUD;
    [SerializeField] private string sceneName = "OfficeWorkplace";

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
        mainMenu.SetActive(false);
        HUD.SetActive(true);

        SceneLoader.Instance.LoadScene(sceneName);
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
