using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class levelFailure : MonoBehaviour
{
    private EmployeeData currentSession;
	private GameObject planetNameObject;
	private GameObject buildNameObject;
	private GameObject stamp1;
	private GameObject stamp2;
	private GameObject stamp3;
	private GameObject deathReason;
	private int levelNumber;
	private string planetName;
	[SerializeField] private Sprite stampHPC;
	[SerializeField] private Sprite stampCMS;
	[SerializeField] private Sprite stampFail;
	[SerializeField] private VoidEventChannelSO triggerMainMenu;
    [SerializeField] private VoidEventChannelSO triggerNextLevel;
	[SerializeField] private VoidEventChannelSO startGame;
	
	public void OnEnable()
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
			if(child.name == "Stamp1")
			{
				stamp1 = child.gameObject;
			}
			if(child.name == "Stamp2")
			{
				stamp2 = child.gameObject;
			}
			if(child.name == "Stamp3")
			{
				stamp3 = child.gameObject;
			}
			if(child.name == "DeathReason")
			{
				deathReason = child.gameObject;
			}
		}
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
			setLevelFailureScreen();
        }
    }
	
	public void setLevelFailureScreen()
	{
		levelNumber = LevelManager.Instance.currentSession.currentDay - 1;
		LevelInfoSO currentLevel = LevelManager.Instance.levels[levelNumber];
		setStamps();
		string buildName = currentLevel.buildName;
		string spaceName = "";
		string termination = "";
		spaceName = spaceName + buildName[0];
		for(int i = 1; i < buildName.Length; i++)
		{
			if(char.IsUpper(buildName[i]) && buildName[i - 1] != ' ')
			{
				spaceName = spaceName + ' ';
			}
			spaceName = spaceName + buildName[i];
		}
		if(LevelManager.Instance.currentSession.efficiency <= 0)
		{
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.PoorEfficiencyLoss);}
			termination = "Poor Efficiency";
		}
		else
		{
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.TooSlowLoss);}
			termination = "Missed Deadlines";
		}
		if(LevelManager.Instance.coffeeUsed == true)
		{
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.CompromisedBuildLoss);}
			termination = "Compromised Build";
		}
		planetNameObject.GetComponent<TMP_Text>().text = planetName;
		buildNameObject.GetComponent<TMP_Text>().text = spaceName;
		deathReason.GetComponent<TMP_Text>().text = termination;
	}
	
	public void PressMainMenuButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		triggerMainMenu.RaiseEvent();
	}
	
	public void PressRetryButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		startGame.RaiseEvent();
		triggerNextLevel.RaiseEvent();
	}
	
	private void setStamps()
	{
		int adjustedNumber = levelNumber + 1;
		if (adjustedNumber >= 1 && adjustedNumber <= 3)
		{
			planetName = "Athanor"; 
			for(int i = 0; i < 3; i++)
			{
				GameObject stamp = null;
				if(i == 0)
				{
					stamp = stamp1;
				}
				else if(i == 1)
				{
					stamp = stamp2;
				}
				else
				{
					stamp = stamp3;
				}
				if(currentSession.levelBuildChoices[i] == 1)
				{
					stamp.GetComponent<Image>().sprite = stampHPC;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else if(currentSession.levelBuildChoices[i] == 2)
				{
					stamp.GetComponent<Image>().sprite = stampCMS;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else{
					stamp.GetComponent<Image>().sprite = null;
					stamp.GetComponent<Image>().color = Color.clear;
				}
				if(levelNumber == i)
				{
					stamp.GetComponent<Image>().sprite = stampFail;
					stamp.GetComponent<Image>().color = new Color(1f, 16f/255f, 112/255f, 1f);
				}
			}
			
		}
		else if (adjustedNumber  >= 4 && adjustedNumber  <= 6)
		{
			planetName = "Melligo"; 
			for(int i = 0; i < 3; i++)
			{
				GameObject stamp = null;
				if(i == 0)
				{
					stamp = stamp1;
				}
				else if(i == 1)
				{
					stamp = stamp2;
				}
				else
				{
					stamp = stamp3;
				}
				if(currentSession.levelBuildChoices[i + 3] == 1)
				{
					stamp.GetComponent<Image>().sprite = stampHPC;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else if(currentSession.levelBuildChoices[i + 3] == 2)
				{
					stamp.GetComponent<Image>().sprite = stampCMS;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else
				{
					stamp.GetComponent<Image>().sprite = null;
					stamp.GetComponent<Image>().color = Color.clear;
				}
				if(levelNumber == (i + 3))
				{
					stamp.GetComponent<Image>().sprite = stampFail;
					stamp.GetComponent<Image>().color = new Color(1f, 16f/255f, 112/255f, 1f);
				}
			}
		}
		else if (adjustedNumber  >= 7 && adjustedNumber  <= 9)
		{
			planetName = "Crasis"; 
			for(int i = 0; i < 3; i++)
			{
				GameObject stamp = null;
				if(i == 0)
				{
					stamp = stamp1;
				}
				else if(i == 1)
				{
					stamp = stamp2;
				}
				else
				{
					stamp = stamp3;
				}
				if(currentSession.levelBuildChoices[i + 6] == 1)
				{
					stamp.GetComponent<Image>().sprite = stampHPC;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else if(currentSession.levelBuildChoices[i + 6] == 2)
				{
					stamp.GetComponent<Image>().sprite = stampCMS;
					stamp.GetComponent<Image>().color = Color.white;
				}
				else{
					stamp.GetComponent<Image>().sprite = null;
					stamp.GetComponent<Image>().color = Color.clear;
				}
				if(levelNumber == (i + 6))
				{
					stamp.GetComponent<Image>().sprite = stampFail;
					stamp.GetComponent<Image>().color = new Color(1f, 16f/255f, 112/255f, 1f);
				}
			}
		}
	}
}
