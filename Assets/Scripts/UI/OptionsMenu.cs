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

        if (transform.Find("AudioPanelCanvas") != null)
        {
            transform.Find("AudioPanelCanvas").gameObject.SetActive(true);
        }

        lastSubMenu = GameObject.Find("AudioPanelButton");
    }

    public void LoadControls()
    {
        Time.timeScale = 1f;
        ResetSubmenus();

        if (transform.Find("ControlsPanelCanvas") != null)
        {
            transform.Find("ControlsPanelCanvas").gameObject.SetActive(true);
        }

        lastSubMenu = GameObject.Find("ControlsPanelButton");
    }

    public void LoadDisplay()
    {
        Time.timeScale = 1f;
        ResetSubmenus();

        if (transform.Find("DisplayPanelCanvas") != null)
        {
            transform.Find("DisplayPanelCanvas").gameObject.SetActive(true);
        }

        lastSubMenu = GameObject.Find("DisplayPanelButton");
    }

    public void ReturnToPreviousScene()
    {
        Time.timeScale = 1f;
        currentMenu.SetActive(false);

        Time.timeScale = 0f;
        lastMenu.gameObject.SetActive(true);
        
    }

    private void RefreshLastSubMenuHighlight()
    {
        if (lastSubMenu == null)
        {
            return;
        }

        var button = lastSubMenu.GetComponent<UnityEngine.UI.Button>();
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
        }
    }
}
