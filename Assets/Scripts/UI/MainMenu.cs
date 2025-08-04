using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject MainMenuCamera;
    private string sceneName = "MainGameOfficeBase";

    public void SetAction(string action)
    {
        Invoke(action, 0.6f);
    }

    private void LoadOptionsMenu()
    {
        OptionsMenuButtons.LastMenu = currentMenu;
        currentMenu.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void LoadGame()
    {
        MainMenuCamera.GetComponent<AudioListener>().enabled = false;
        currentMenu.SetActive(false);

        SceneLoader.Instance.LoadScene(sceneName);
        Invoke(nameof(DisableMainMenuCamera), 0.5f);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void DisableMainMenuCamera()
    {
        MainMenuCamera.gameObject.SetActive(false);
    }
}
