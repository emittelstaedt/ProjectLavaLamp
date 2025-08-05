using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject[] subMenus;
    private static GameObject lastMenu;
    private static GameObject lastSubMenu;

    public static GameObject LastSubMenu => lastSubMenu;
    public static GameObject LastMenu
    {
        private get => lastMenu;
        set => lastMenu = value;
    }

    private void Awake()
    {
        lastSubMenu = GameObject.Find("AudioPanelButton");
    }

    private void OnEnable()
    {
        RefreshLastSubMenuHighlight();
    }

    private void Update()
    {
        // This looks at which submenu is open and sets a bool to create a static appearing animation for the associated button.
        foreach (GameObject menu in subMenus)
        {
            if (menu.activeSelf)
            {
                if (EventSystem.current.currentSelectedGameObject != lastSubMenu && IsSelectable(EventSystem.current.currentSelectedGameObject))
                {
                    SetStaticSelected(lastSubMenu, false);

                    lastSubMenu = EventSystem.current.currentSelectedGameObject;
                    SetStaticSelected(lastSubMenu, true);
                }
            }
        }
    }

    private bool IsSelectable(GameObject selectable)
    {
        if (selectable == null)
        {
            return false;
        }

        string name = selectable.name;
        return name == "ControlsPanelButton"
            || name == "AudioPanelButton"
            || name == "DisplayPanelButton";
    }

    private void ResetSubmenus()
    {
        foreach (GameObject menu in subMenus)
        {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
            }
        }
    }

    public void LoadAudio()
    {
        Time.timeScale = 1f;
        ResetSubmenus();

        transform.Find("AudioPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("AudioPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    public void LoadControls()
    {
        Time.timeScale = 1f;
        ResetSubmenus();

        transform.Find("ControlsPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("ControlsPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    public void LoadDisplay()
    {
        Time.timeScale = 1f;
        ResetSubmenus();

        transform.Find("DisplayPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("DisplayPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    public void ReturnToPreviousScene()
    {
        Time.timeScale = 1f;
        currentMenu.SetActive(false);

        loadingCanvas.SetActive(true);

        if (LastSubMenu != null)
        {
            SetStaticSelected(lastSubMenu, false);
        }
        Time.timeScale = 0f;
        lastMenu.gameObject.SetActive(true);
        
    }

    private void SetStaticSelected(GameObject selectedButton, bool active)
    {
        if (selectedButton == null)
        {
            Time.timeScale = 0f;
            return;
        }

        Animator selectedAnimator = selectedButton.GetComponent<Animator>();
        if (selectedAnimator != null)
        {
            selectedAnimator.SetBool("isActiveButton", active);
        }
        Time.timeScale = 0f;
    }

    private void RefreshLastSubMenuHighlight()
    {
        SetStaticSelected(lastSubMenu, true);
    }
}
