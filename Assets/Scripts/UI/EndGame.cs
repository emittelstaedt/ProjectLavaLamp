using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class EndGame : MonoBehaviour
{
	private EmployeeData currentSession;
	private GameObject endGameTitleObject;
	[SerializeField] private GameObject[] stamps;
	[SerializeField] private Sprite stampHPC;
	[SerializeField] private Sprite stampCMS;
	[SerializeField] private VoidEventChannelSO triggerMainMenu;
	private GameObject score;
	private GameObject buildTime;
	private int[] ScienceEnding;
	private int[] BadScienceEnding;
	private int[] HPCEnding;
	private int[] CMSEnding;
	//private int givenEnding; //0 neutral 1 correct science 2 incorrect science 3 hpc 4 cms 
	
	public void OnEnable()
	{
		ScienceEnding = new int[] {1, 2, 2, 2, 1, 2, 1, 2, 2};
		BadScienceEnding = new int[] {1, 1, 1, 1, 2, 1, 2, 1, 1};
		HPCEnding = new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1};
		CMSEnding = new int[] {1, 2, 2, 2, 2, 2, 2, 2, 2};
		//givenEnding = 0;
		
		foreach(Transform child in gameObject.GetComponentsInChildren<Transform>())
		{
			if(child.name == "GameResultText")
			{
				endGameTitleObject = child.gameObject;
			}
			if(child.name == "EfficiencyScore")
			{
				score = child.gameObject;
			}
			if(child.name == "BuildTime")
			{
				buildTime = child.gameObject;
			}
		}
		if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
			setEndGameScreen();
        }
	}
	
	public void setEndGameScreen()
	{
		string endTitle = "Quiet Quitter";
		Color endColor = new Color(186f/255f, 189f/255f, 189f/255f, 1f);
		if(compareArrays(currentSession.levelBuildChoices, ScienceEnding))
		{
			currentSession.endings[1] = true;
			endTitle = "Spectacular Scientist";
			endColor = new Color(47f/255f, 213f/255f, 205f/255f, 1f);
		}
		else if(compareArrays(currentSession.levelBuildChoices, BadScienceEnding))
		{
			currentSession.endings[2] = true;
			endTitle = "Antiscience Anti-Superstar";
			endColor = new Color(1f, 135f/255f, 65f/255f, 1f);
		}
		else if(compareArrays(currentSession.levelBuildChoices, HPCEnding))
		{
			currentSession.endings[3] = true;
			endTitle = "HPC Superstar";
			endColor = new Color(1f, 0f, 0f, 1f);
		}
		else if(compareArrays(currentSession.levelBuildChoices, CMSEnding))
		{
			currentSession.endings[4] = true;
			endTitle = "CMS Believer";
			endColor = new Color(240f/255f, 212f/255f, 57f/255f, 1f);
		}
		else
		{
			currentSession.endings[0] = true;
		}
		setStamps();
		float averageTime = 0f;
		for(int i = 0; i < 9; i++)
		{
			averageTime += LevelManager.Instance.currentSession.levelCompleteTimes[i];
		}
		averageTime = averageTime / 9f;
		int minutes = (int)(averageTime / 60);
		int seconds = (int)(averageTime - (minutes * 60));
		int milliseconds = (int)(Mathf.Floor(1000f * (averageTime % 1f)));
		string timeDisplayed = minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
		buildTime.GetComponent<TMP_Text>().text = timeDisplayed;
		score.GetComponent<TMP_Text>().text = LevelManager.Instance.currentSession.efficiency.ToString();
		endGameTitleObject.GetComponent<TMP_Text>().text = endTitle;
		endGameTitleObject.GetComponent<TMP_Text>().color = endColor;
		//Reset signifcant values in current save file and then save
		for(int i = 0; i < currentSession.levelBuildChoices.Length; i++)
		{
			currentSession.levelBuildChoices[i] = 0;
			currentSession.levelCompleteTimes[i] = 0f;
		}
		currentSession.coffeeLevel = 3;
		currentSession.efficiency = 1000;
		if (LevelManager.Instance != null)
        {
			LevelManager.Instance.saveGame();
		}
	}
	
	public void PressMainMenuButton()
	{

		triggerMainMenu.RaiseEvent();
	}
	
	private void setStamps()
	{
		for(int i = 0; i < 9; i++)
		{
			if(currentSession.levelBuildChoices[i] == 1)
			{
				stamps[i].GetComponent<Image>().sprite = stampHPC;
				stamps[i].GetComponent<Image>().color = Color.white;
			}
			else if(currentSession.levelBuildChoices[i] == 2)
			{
				stamps[i].GetComponent<Image>().sprite = stampCMS;
				stamps[i].GetComponent<Image>().color = Color.white;
			}
			else
			{
				stamps[i].GetComponent<Image>().sprite = null;
				stamps[i].GetComponent<Image>().color = Color.clear;
			}
		}
	}
	
	//This assumes the arrays are the same size
	private bool compareArrays(int[] array1, int[] array2)
	{
		for(int i = 0; i < array1.Length; i++)
		{
			if(array1[i] != array2[i])
			{
				return false;
			}
		}
		return true;
	}
}
