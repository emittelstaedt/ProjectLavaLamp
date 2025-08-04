using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject mainMenu;
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

    public void SetAction(string action)
    {
        Time.timeScale = 1f;
        Invoke(action, 0.6f);
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

    private void LoadAudio()
    {
        ResetSubmenus();

        transform.Find("AudioPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("AudioPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    private void LoadControls()
    {
        ResetSubmenus();

        transform.Find("ControlsPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("ControlsPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    private void LoadDisplay()
    {
        ResetSubmenus();

        transform.Find("DisplayPanelCanvas")?.gameObject.SetActive(true);
        lastSubMenu = GameObject.Find("DisplayPanelButton");
        SetStaticSelected(lastSubMenu, true);
    }

    private void ReturnToPreviousScene()
    {
        currentMenu.SetActive(false);
        lastMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
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
