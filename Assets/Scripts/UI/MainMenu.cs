using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject MainMenuCamera;
    [SerializeField] private GameObject loadingCanvas;
    private string sceneName = "MainGameOfficeBase";

    private void OnEnable()
    {
        Time.timeScale = 1f;
    }

    public void LoadOptionsMenu()
    {
        OptionsMenuButtons.LastMenu = currentMenu;
        loadingCanvas.SetActive(true);
        currentMenu.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    public void LoadGame()
    {
        loadingCanvas.SetActive(true);

        MainMenuCamera.GetComponent<AudioListener>().enabled = false;
        currentMenu.SetActive(false);

        SceneLoader.Instance.LoadScene(sceneName);
        MainMenuCamera.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
