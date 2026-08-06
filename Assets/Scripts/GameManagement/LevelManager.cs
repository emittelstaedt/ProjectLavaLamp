using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private EmployeeDataEventChannelSO sendEmployeeID;
	[SerializeField] private BoolEventChannelSO setCursorVisibility;
	[SerializeField] private IntEventChannelSO setProfilePointer;
	[SerializeField] private LevelInfoSOEventChannelSO sendLevel;
	[SerializeField] private EmailEventChannelSO sendEmail;
	[SerializeField] private VoidEventChannelSO planetCheck;
	[SerializeField] private IntEventChannelSO modifyValue;
	[SerializeField] private VoidEventChannelSO stopInteract;
	[SerializeField] private email efficiencyMail;
	[SerializeField] private GameObject memo;
	[SerializeField] private GameObject newspaper;	
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private GameObject startMenu;
	[SerializeField] private GameObject menuButtons;
	[SerializeField] private GameObject menuLogo;
	[SerializeField] private GameObject HUD;
	[SerializeField] private GameObject levelSuccess;
	[SerializeField] private GameObject levelFailure;
	[SerializeField] private GameObject endGame;
	[SerializeField] private GameObject credits;
	[SerializeField] private GameObject confirmNamePanel;
    public LevelInfoSO[] levels;
	public EmployeeData[] profiles;
	public string[] profilePaths;
	private float startTime;
	private float endTime;
	private float totalTime;
	private float profileStartTime;
	private float profileEndTime;
	private bool CMSUsed;
	public bool coffeeUsed;
	private int lossCounter; 
	
	public EmployeeData currentSession;
	
	public static LevelManager Instance = null;
    
	private void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<LevelManager>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
		profiles = new EmployeeData[6];
		profilePaths = new string[6];
		for(int i = 0; i < 6; i++)
		{
			profilePaths[i] = Application.persistentDataPath + "/employee" + i.ToString() + ".json"; 
		}
		loadGame();
		lossCounter = 0;
    }
	
	public void checkProfileSelection()
	{
		StartCoroutine(checkProfileSelectionCoroutine());
	}
	
	public IEnumerator checkProfileSelectionCoroutine()
	{
		yield return null;
		if(profiles[0].employeeName != "")
		{
			currentSession = profiles[0];
			setProfilePointer.RaiseEvent(0);
		}
		else if(profiles[1].employeeName != "")
		{
			currentSession = profiles[1];
			setProfilePointer.RaiseEvent(1);
		}
		else if(profiles[2].employeeName != "")
		{
			currentSession = profiles[2];
			setProfilePointer.RaiseEvent(2);
		}
		else if(profiles[3].employeeName != "")
		{
			currentSession = profiles[3];
			setProfilePointer.RaiseEvent(3);
		}
		else if(profiles[4].employeeName != "")
		{
			currentSession = profiles[4];
			setProfilePointer.RaiseEvent(4);
		}
		else if(profiles[5].employeeName != "")
		{
			currentSession = profiles[5];
			setProfilePointer.RaiseEvent(5);
		}
		else
		{
			setProfilePointer.RaiseEvent(-1);
		}
	}
	public void saveGame()
	{
		int employeeNumber = currentSession.employeeNumber;
		string json = JsonUtility.ToJson(currentSession);
		File.WriteAllText(profilePaths[employeeNumber], json);
	}
	
	private void loadGame(){
		
		for(int i = 0; i < 6; i++)
		{
			EmployeeData employeeProfile = new EmployeeData();
			if(File.Exists(profilePaths[i]))
			{
				string json = File.ReadAllText(profilePaths[i]);
				employeeProfile = JsonUtility.FromJson<EmployeeData>(json);
			}
			profiles[i] = employeeProfile;
			profiles[i].employeeNumber = i;
			
		}
	}
	
	public void displayEmployeeIDs()
	{
		StartCoroutine(nameProfiles());
	}
	
	private IEnumerator nameProfiles()
	{
		yield return null;
		for(int i = 0; i < 6; i++)
		{
			sendEmployeeID.RaiseEvent(profiles[i]);
		}
	}
	
	public void selectEmployee(int employeeNumber)
	{
		currentSession = profiles[employeeNumber];
		if(profiles[employeeNumber].employeeName == "")
		{
			confirmNamePanel.SetActive(true);
		}
		else
		{
			setProfilePointer.RaiseEvent(employeeNumber);
		}
	}
	
	public void setEmployeeName(string employeeName)
	{
		currentSession.employeeName = employeeName;
		saveGame();
		sendEmployeeID.RaiseEvent(currentSession);
	}
	
	public void deleteEmployee(){
		EmployeeData blankEmployee = new EmployeeData();
		int fileNumber = currentSession.employeeNumber;
		currentSession = blankEmployee;
		currentSession.employeeNumber = fileNumber;
		profiles[currentSession.employeeNumber] = currentSession;
		File.Delete(profilePaths[currentSession.employeeNumber]);
		displayEmployeeIDs();
	}
	
	public void resetEmployee()
	{
		currentSession.currentDay = 1;
		for(int i = 0; i < currentSession.levelBuildChoices.Length; i++)
		{
			currentSession.levelBuildChoices[i] = 0;
			currentSession.levelCompleteTimes[i] = 0f;
		}
		currentSession.coffeeLevel = 3;
		currentSession.efficiency = 500;
	}
	
	public void pushLevel(){
		sendLevel.RaiseEvent(levels[currentSession.currentDay - 1]);
	}
	
	public void levelComplete()
	{
		//Call corresponding level complete achievement
		if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(currentSession.currentDay-1);} //Minus 1 since enum for it is 0-8
		//Check for completing day under 100 efficiency achievement
		if(currentSession.efficiency<100){
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.BeatDayUnder100);}
		}

		lossCounter = 0;
		endTime = Time.realtimeSinceStartup;
		totalTime = endTime - startTime;
		if((((float)totalTime)/60f)<3f){ //Build in under 3 minutes achievement
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.BuildUnder3Mins);}
		}
		currentSession.levelCompleteTimes[currentSession.currentDay - 1] = totalTime;
		if(currentSession.currentDay != levels.Length)
		{
			currentSession.currentDay++;
		}else{
			currentSession.currentDay = 1;
		}
		saveGame();
		StartCoroutine(PauseBeforeLevelSuccess());
    }
	
	public void levelIncomplete()
	{
		lossCounter++;
		StartCoroutine(PauseBeforeLevelFailure());
	}
	
	public void activateStartMenu()
	{
		StartCoroutine(ReturnToStartMenu());
	}
	
	public void activateNextLevel()
	{
		StartCoroutine(ContinueToNextLevel());
	}
	
	public void activateEndGame()
	{
		StartCoroutine(ContinueToEndGame());
	}
	
	public void activateCredits()
	{
		StartCoroutine(ContinueToCredits());
	}
	
	private IEnumerator ReturnToStartMenu()
	{
		loadGame();
		loadingScreen.SetActive(true);
		levelSuccess.SetActive(false);
		levelFailure.SetActive(false);
		endGame.SetActive(false);
		credits.SetActive(false);
		Animator buttonAnimator = menuButtons.GetComponent<Animator>();
		Animator logoAnimator = menuLogo.GetComponent<Animator>();
		if(SceneLoader.Instance.IsSceneLoaded("OfficeWorkplace") == true)
		{
			SceneLoader.Instance.UnloadScene("OfficeWorkplace");
		}
		yield return null;
		startMenu.SetActive(true);
		buttonAnimator.SetTrigger("Return");
		logoAnimator.SetTrigger("Return");
		stopProfileGameTime();
	}
	
	private IEnumerator ContinueToNextLevel()
	{
		CMSUsed = false;
		coffeeUsed = false;
		loadGame();
		currentSession = profiles[currentSession.employeeNumber];
		if(currentSession.coffeeLevel != 0)
		{
			currentSession.coffeeLevel--;
		}
		loadingScreen.SetActive(true);
		levelSuccess.SetActive(false);
		levelFailure.SetActive(false);
		planetCheck.RaiseEvent();
		if(SceneLoader.Instance.IsSceneLoaded("OfficeWorkplace"))
		{
			SceneLoader.Instance.UnloadScene("OfficeWorkplace");
		}
		yield return null;
		HUD.SetActive(true);
		SceneLoader.Instance.LoadScene("OfficeWorkplace");
		InputSystem.actions.FindActionMap("Player").Enable();
		startTime = Time.realtimeSinceStartup;
		if(lossCounter >= 3)
		{
			StartCoroutine(efficiencyDelivery());
		}
	}
	
	private IEnumerator ContinueToEndGame()
	{
		loadingScreen.SetActive(true);
		levelSuccess.SetActive(false);
		levelFailure.SetActive(false);
		planetCheck.RaiseEvent();
		SceneLoader.Instance.UnloadScene("OfficeWorkplace");
		yield return null;
		endGame.SetActive(true);
	}
	
	private IEnumerator ContinueToCredits()
	{
		loadingScreen.SetActive(true);
		endGame.SetActive(false);
		yield return null;
		credits.SetActive(true);
	}
	
	private IEnumerator PauseBeforeLevelSuccess()
	{
		yield return new WaitForSeconds(0.5f);
		memo.SetActive(false);
		newspaper.SetActive(false);
		levelSuccess.SetActive(true);
		HUD.SetActive(false);
		setCursorVisibility.RaiseEvent(true);
		InputSystem.actions.FindActionMap("Player").Disable();
	}
	
	private IEnumerator PauseBeforeLevelFailure()
	{
		yield return new WaitForSeconds(0.5f);
		memo.SetActive(false);
		newspaper.SetActive(false);
		levelFailure.SetActive(true);
		stopInteract.RaiseEvent();
		HUD.SetActive(false);
		setCursorVisibility.RaiseEvent(true);
		InputSystem.actions.FindActionMap("Player").Disable();
	}
	
	public void startProfileGameTime()
	{
		profileStartTime = Time.realtimeSinceStartup;
	}
	
	public void stopProfileGameTime()
	{
		profileEndTime = Time.realtimeSinceStartup;
		float additionalGameTime = profileEndTime - profileStartTime;
		loadGame();
		currentSession = profiles[currentSession.employeeNumber];
		currentSession.totalGameTime += additionalGameTime;
		saveGame();
	}
	
	public void CMSPlaced()
	{
		CMSUsed = true;
	}
	
	public void coffeeDrank()
	{
		if(CMSUsed == true)
		{
			coffeeUsed = true;
			StartCoroutine(delayedDisappear());
		}
	}
	
	public IEnumerator delayedDisappear()
	{
		yield return new WaitForSeconds(1.5f);
		levelIncomplete();
	}
	
	private IEnumerator efficiencyDelivery()
	{
		yield return new WaitForSeconds(0.25f);

		if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.GetEfficiencyPity);}
		
		sendEmail.RaiseEvent(efficiencyMail);
		modifyValue.RaiseEvent(200);

		if(currentSession.efficiency>=1000){
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.GetMaxEfficiencyPity);}
		}

		saveGame();
		lossCounter = 0;
	}
}
