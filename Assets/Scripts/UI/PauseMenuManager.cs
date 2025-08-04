using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
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

        if (!optionsMenu.activeInHierarchy && !mainMenu.activeInHierarchy)
        {
            isOpen = !isOpen;
            pauseMenu.SetActive(isOpen);

            if (isOpen)
            {
                Time.timeScale = 0f;
                InputSystem.actions.FindActionMap("Player").Disable();
            }
            else
            {
                Time.timeScale = 1f;
                InputSystem.actions.FindActionMap("Player").Enable();
                wasUnpaused = false;
            }
        }
    }
}
