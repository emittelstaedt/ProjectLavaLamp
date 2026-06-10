using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private EmployeeDataEventChannelSO sendEmployeeID;
	[SerializeField] private VoidEventChannelSO enableButtons;
	[SerializeField] private GameObject profileNamePanel;
	[SerializeField] private LevelInfoSO[] levels;
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
	
}
