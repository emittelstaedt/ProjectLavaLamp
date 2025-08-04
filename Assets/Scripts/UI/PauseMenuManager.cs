using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool wasUnpaused;
    private string sceneName = "MainGameOfficeBase";
    private bool isOpen = false;

    public static PauseMenuManager Instance = null;
    public bool WasUnpaused
    {
        private get => wasUnpaused;
        set => wasUnpaused = value;
    }

    private void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<PauseMenuManager>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!SceneLoader.Instance.IsSceneLoaded(sceneName))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || wasUnpaused)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!SceneLoader.Instance.IsSceneLoaded(sceneName))
        {
            return;
        }

        isOpen = !isOpen;
        pauseMenu.SetActive(isOpen);

        if (isOpen)
        {
            InputSystem.actions.FindActionMap("Player").Disable();
        }
        else
        {
            InputSystem.actions.FindActionMap("Player").Enable();
            wasUnpaused = false;
        }
    }

    public void ResumeGame()
    {
        if (isOpen)
        {
            TogglePause();
        }
    }
}
