using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private EmployeeDataEventChannelSO sendEmployeeID;
	[SerializeField] private VoidEventChannelSO enableButtons;
	[SerializeField] private BoolEventChannelSO setCursorVisibility;
	[SerializeField] private LevelInfoSOEventChannelSO sendLevel;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject HUD;
	[SerializeField] private GameObject levelSuccess;
	[SerializeField] private GameObject profileNamePanel;
	public LevelInfoSO[] levels;
	[SerializeField] private EmployeeData[] profiles;
	[SerializeField] private string[] profilePaths;
	
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
    }
	
	private void saveGame()
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
			profileNamePanel.SetActive(true);
		}
		enableButtons.RaiseEvent();
		
	}
	
	public void setEmployeeName(string employeeName)
	{
		currentSession.employeeName = employeeName;
		saveGame();
		sendEmployeeID.RaiseEvent(currentSession);
		enableButtons.RaiseEvent();
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
	
	public void pushLevel(){
		sendLevel.RaiseEvent(levels[currentSession.currentDay - 1]);
	}
	
	public void levelComplete(){
		if(currentSession.currentDay != levels.Length)
		{
			currentSession.currentDay++;
		}else{
			currentSession.currentDay = 1;
		}
		saveGame();
		HUD.SetActive(false);
		setCursorVisibility.RaiseEvent(true);
		InputSystem.actions.FindActionMap("Player").Disable();
        InputSystem.actions.FindAction("Pause").Enable();
	    StartCoroutine(PauseBeforeLevelSuccess());
    }

	public void activateMainMenu()
	{
		StartCoroutine(ReturnToMainMenu());
	}
	
	public void activateNextLevel()
	{
		StartCoroutine(ContinueToNextLevel());
	}
	
	private IEnumerator ReturnToMainMenu()
	{
		loadingScreen.SetActive(true);
		levelSuccess.SetActive(false);
		SceneLoader.Instance.UnloadScene("OfficeWorkplace");
		yield return null;
		mainMenu.SetActive(true);
	}
	
	private IEnumerator ContinueToNextLevel()
	{
		loadingScreen.SetActive(true);
		levelSuccess.SetActive(false);
		SceneLoader.Instance.UnloadScene("OfficeWorkplace");
		InputSystem.actions.FindAction("Pause").Disable();
		yield return null;
		SceneLoader.Instance.LoadScene("OfficeWorkplace");
		InputSystem.actions.FindActionMap("Player").Enable();
		HUD.SetActive(true);
		
	}
	
	private IEnumerator PauseBeforeLevelSuccess()
	{
		yield return new WaitForSeconds(0.5f);
		levelSuccess.SetActive(true);
	}
}
