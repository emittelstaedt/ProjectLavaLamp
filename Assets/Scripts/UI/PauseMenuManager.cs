using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject HUD;
    private InputAction pauseAction;
    private bool wasUnpaused;
    private string sceneName = "MainGameOfficeBase";

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

        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    void Update()
    {
        if (!SceneLoader.Instance.IsSceneLoaded(sceneName))
        {
            return;
        }

        if (pauseAction.WasPressedThisFrame() || wasUnpaused)
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

        if (!optionsMenu.activeInHierarchy && !mainMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 0f;
                HUD.SetActive(false);
                InputSystem.actions.FindActionMap("Player").Disable();
                InputSystem.actions.FindAction("Pause").Enable();
            }
            else
            {
                Time.timeScale = 1f;
                HUD.SetActive(true);
                InputSystem.actions.FindActionMap("Player").Enable();
                wasUnpaused = false;
            }
        }
    }
}
