using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ProfileMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
	[SerializeField] private GameObject profileMenu;
    [SerializeField] private GameObject optionsMenu;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private GameObject confirmTrashPanel;
	[SerializeField] private GameObject confirmNamePanel;
	[SerializeField] private GameObject confirmResetPanel;
	[SerializeField] private GameObject noProfilePanel;
	[SerializeField] private GameObject header;
	[SerializeField] private GameObject profileDetailPanel;
	[SerializeField] private TMP_InputField profileField;
	[SerializeField] private TMP_Text placementField;
	[SerializeField] private GameObject footer;
	[SerializeField] private StringEventChannelSO sendProfileName;
	[SerializeField] private VoidEventChannelSO displayIDs;
	[SerializeField] private VoidEventChannelSO checkProfileSelection;
	[SerializeField] private IntEventChannelSO setProfilePointer;
	[SerializeField] private VoidEventChannelSO deleteProfile;
	[SerializeField] private VoidEventChannelSO resetProfile;
	[SerializeField] private VoidEventChannelSO startGame;
	[SerializeField] private VoidEventChannelSO triggerNextLevel;
	[SerializeField] private VoidEventChannelSO startProfileTimer;
	[SerializeField] private VoidEventChannelSO achievementCheck;
    [SerializeField] private GameObject HUD;
	private EmployeeData previousSession;
	
	public void Update()
	{
		if(profileField.isFocused)
		{
			placementField.text = "";
		}
		else
		{
			placementField.text = "What is your name?";
		}
	}
	
    private void OnEnable()
    {
        Time.timeScale = 1f;
		displayIDs.RaiseEvent();
		checkProfileSelection.RaiseEvent();
    }

	public void BackButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		profileMenu.SetActive(false);
	}
	
    public void LoadGame()
    {
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		startMenu.SetActive(false);
        profileMenu.SetActive(false);
		startGame.RaiseEvent();
		triggerNextLevel.RaiseEvent();
		startProfileTimer.RaiseEvent();
    }
	
	public void NameProfile()
    {
		profileField.text = "";
    }

    public void ConfirmProfileName()
    {
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		if(profileField.text != "")
		{
			previousSession = LevelManager.Instance.currentSession;
			sendProfileName.RaiseEvent(profileField.text);
			displayProfile(LevelManager.Instance.currentSession.employeeNumber);
			setProfilePointer.RaiseEvent(LevelManager.Instance.currentSession.employeeNumber);
			confirmNamePanel.SetActive(false);
		}
    }

    public void CancelProfileName()
    {
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		LevelManager.Instance.currentSession = previousSession;
        confirmNamePanel.SetActive(false);
    }
	
	public void TrashProfile()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		confirmTrashPanel.SetActive(true);
	}
	
	public void CancelTrash()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		confirmTrashPanel.SetActive(false);
	}
	
	public void ConfirmTrash()
	{	
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		deleteProfile.RaiseEvent();
		displayProfile(-1);
		setProfilePointer.RaiseEvent(-1);
		confirmTrashPanel.SetActive(false);
	}
	
	public void ResetProfile()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		confirmResetPanel.SetActive(true);
	}
	
	public void CancelReset()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		confirmResetPanel.SetActive(false);
	}
	
	public void ConfirmReset()
	{	
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		resetProfile.RaiseEvent();
		displayProfile(LevelManager.Instance.currentSession.employeeNumber);
		setProfilePointer.RaiseEvent(LevelManager.Instance.currentSession.employeeNumber);
		LevelManager.Instance.saveGame();
		confirmResetPanel.SetActive(false);
	}
	
	public void displayProfile(int selection)
	{
		if(selection != -1)
		{
			if(LevelManager.Instance.currentSession.employeeName != "")
			{
				noProfilePanel.SetActive(false);
				Transform planetDay = profileDetailPanel.transform.Find("PlanetDay");
				foreach(Transform child in planetDay)
				{
					if(child.name == "Planet (TMP)")
					{
						TMP_Text planet = child.gameObject.GetComponent<TMP_Text>();
						string displayPlanet = "Planet: ";
						if(LevelManager.Instance.currentSession.currentDay <= 3)	
						{
							displayPlanet = displayPlanet + "Athanor";
						}
						else if(LevelManager.Instance.currentSession.currentDay <= 6)
						{
							displayPlanet = displayPlanet + "Melligo";
						}
						else
						{
							displayPlanet = displayPlanet + "Crasis";
						}
						planet.text = displayPlanet;
					}
					if(child.name == "Day (TMP)")
					{
						TMP_Text day = child.gameObject.GetComponent<TMP_Text>();
						string displayDay = "Day " + LevelManager.Instance.currentSession.currentDay;
						day.text = displayDay;
					}
				}
				Transform Hours = profileDetailPanel.transform.Find("Hours");
				foreach(Transform child in Hours)
				{
					if(child.name == "Timer (TMP)")
					{
						TMP_Text time = child.gameObject.GetComponent<TMP_Text>();
						float totalTime = LevelManager.Instance.currentSession.totalGameTime;
						int hours = (int)(totalTime / 3600);
						int minutes = (int)((totalTime / 60f) - (hours * 60));
						int seconds = (int)(totalTime - (hours * 3600) - (minutes * 60));
						int milliseconds = (int)(Mathf.Floor(1000f * (totalTime % 1f)));
						string timeDisplayed = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
						time.text = timeDisplayed;
					}
				}
				achievementCheck.RaiseEvent();
				
				header.SetActive(true);
				profileDetailPanel.SetActive(true);
				footer.SetActive(true);
			}
		}
		else
		{
			header.SetActive(false);
			profileDetailPanel.SetActive(false);
			footer.SetActive(false);
			noProfilePanel.SetActive(true);
		}
	}
}
