using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSuccess : MonoBehaviour
{
    private EmployeeData currentSession;
	private GameObject planetNameObject;
	private GameObject buildNameObject;
	private GameObject stamp1;
	private GameObject stamp2;
	private GameObject stamp3;
	private GameObject score;
	private GameObject buildTime;
	private TMP_Text nextButton;
	private int levelNumber;
	private string planetName;
	[SerializeField] private Sprite stampHPC;
	[SerializeField] private Sprite stampCMS;
	[SerializeField] private VoidEventChannelSO triggerMainMenu;
    [SerializeField] private VoidEventChannelSO triggerNextLevel;
	[SerializeField] private VoidEventChannelSO triggerEndGame;
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
			if(child.name == "EfficiencyScore")
			{
				score = child.gameObject;
			}
			if(child.name == "BuildTime")
			{
				buildTime = child.gameObject;
			}
			if(child.name == "NextLevel")
			{
				nextButton = child.gameObject.GetComponentInChildren<TMP_Text>();
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
		if(LevelManager.Instance.currentSession.currentDay == 1) //It turned over due to the game looping but they just completed the end
		{
			levelNumber = LevelManager.Instance.levels.Length - 1;
			nextButton.text = "Continue";
		}else
		{
			levelNumber = LevelManager.Instance.currentSession.currentDay - 2;
			nextButton.text = "Next Level";
		}
		LevelInfoSO currentLevel = LevelManager.Instance.levels[levelNumber];
		setStamps();
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
		float levelTime = LevelManager.Instance.currentSession.levelCompleteTimes[levelNumber];
		int minutes = (int)(levelTime / 60);
		int seconds = (int)(levelTime - (minutes * 60));
		int milliseconds = (int)(Mathf.Floor(1000f * (levelTime % 1f)));
		string timeDisplayed = minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
		buildTime.GetComponent<TMP_Text>().text = timeDisplayed;
		score.GetComponent<TMP_Text>().text = LevelManager.Instance.currentSession.efficiency.ToString();
		planetNameObject.GetComponent<TMP_Text>().text = planetName;
		buildNameObject.GetComponent<TMP_Text>().text = spaceName;
	}
	
	public void PressMainMenuButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		triggerMainMenu.RaiseEvent();
	}
	
	public void PressNextLevelButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		if(LevelManager.Instance.currentSession.currentDay == 1)
		{
			triggerEndGame.RaiseEvent();
		}
		else
		{
			startGame.RaiseEvent();
			triggerNextLevel.RaiseEvent();

		}
		
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
			}
		}
	}
}
