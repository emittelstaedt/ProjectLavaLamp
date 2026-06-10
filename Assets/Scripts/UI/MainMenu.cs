using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject confirmQuitPanel;
	[SerializeField] private GameObject confirmTrashPanel;
	[SerializeField] private GameObject profileNamePanel;
	[SerializeField] private StringEventChannelSO sendProfileName;
	[SerializeField] private VoidEventChannelSO displayIDs;
	[SerializeField] private VoidEventChannelSO deleteProfile;
    [SerializeField] private GameObject HUD;
    [SerializeField] private string sceneName = "OfficeWorkplace";
	
    private void OnEnable()
    {
        Time.timeScale = 1f;
		displayIDs.RaiseEvent();
    }

    public void LoadOptionsMenu()
    {
        optionsMenu.SetActive(true);
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
        confirmQuitPanel.SetActive(true);
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
	
	public void NameProfile()
    {
		TMP_InputField profileField = profileNamePanel.GetComponentInChildren<TMP_InputField>();
		profileField.text = "Enter your name here";
		
    }

    public void ConfirmProfileName()
    {
		TMP_InputField profileField = profileNamePanel.GetComponentInChildren<TMP_InputField>();
		sendProfileName.RaiseEvent(profileField.text);
		profileNamePanel.SetActive(false);
    }

    public void CancelProfileName()
    {
        profileNamePanel.SetActive(false);
    }
	
	public void TrashProfile()
	{
		confirmTrashPanel.SetActive(true);
	}
	
	public void CancelTrash()
	{
		confirmTrashPanel.SetActive(false);
	}
	
	public void ConfirmTrash()
	{	
		deleteProfile.RaiseEvent();
		confirmTrashPanel.SetActive(false);
	}
}
