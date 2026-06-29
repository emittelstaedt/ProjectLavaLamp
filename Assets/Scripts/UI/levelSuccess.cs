using UnityEngine;
using TMPro;

public class levelSuccess : MonoBehaviour
{
    private EmployeeData currentSession;
	private GameObject planetNameObject;
	private GameObject buildNameObject;
	[SerializeField] private VoidEventChannelSO triggerMainMenu;
    [SerializeField] private VoidEventChannelSO triggerNextLevel;
	[SerializeField] private VoidEventChannelSO startGame;
	
	void OnEnable()
    {
		foreach(Transform child in gameObject.GetComponentsInChildren<Transform>())
		{
			if(child.name == "PlanetNameText")
			{
				planetNameObject = child.gameObject;
			}
			if(child.name == "BuildNameText")
			{
				buildNameObject = child.gameObject;
			}
		}
          if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
			setLevelSuccessScreen();
        }
    }
	
	public void setLevelSuccessScreen()
	{
		string planetName = "";
		int levelNumber;
		if(LevelManager.Instance.currentSession.currentDay - 2 == -1)
		{
			levelNumber = LevelManager.Instance.levels.Length - 1;
		}else
		{
			levelNumber = LevelManager.Instance.currentSession.currentDay - 2;
		}
		LevelInfoSO currentLevel = LevelManager.Instance.levels[levelNumber];
		if (LevelManager.Instance.currentSession.currentDay >= 1 && LevelManager.Instance.currentSession.currentDay <= 3)
		{
			planetName = "Planet 1"; 
		}else if (LevelManager.Instance.currentSession.currentDay >= 4 && LevelManager.Instance.currentSession.currentDay <= 6)
		{
			planetName = "Planet 2"; 
		}else if (LevelManager.Instance.currentSession.currentDay >= 7 && LevelManager.Instance.currentSession.currentDay <= 9)
		{
			planetName = "Planet 3"; 
		}
		string buildName = currentLevel.buildName;
		string spaceName = "";
		spaceName = spaceName + buildName[0];
		for(int i = 1; i < buildName.Length; i++)
		{
			if(char.IsUpper(buildName[i]) && buildName[i - 1] != ' ')
			{
				spaceName = spaceName + ' ';
			}
			spaceName = spaceName + buildName[i];
		}
		planetNameObject.GetComponent<TMP_Text>().text = planetName;
		buildNameObject.GetComponent<TMP_Text>().text = spaceName;
	}
	
	public void PressMainMenuButton()
	{
		triggerMainMenu.RaiseEvent();
	}
	
	public void PressNextLevelButton()
	{
		triggerNextLevel.RaiseEvent();
		startGame.RaiseEvent();
	}
}
