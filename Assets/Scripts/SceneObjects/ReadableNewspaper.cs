using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class ReadableNewspaper : MonoBehaviour
{
	private EmployeeData currentSession;
	[SerializeField] private newsPaperOptions[] NewsPapers;
	[SerializeField] private GameObject newsPaperName;
	[SerializeField] private GameObject newsPaperTagline;
	[SerializeField] private GameObject articleTitle;
	[SerializeField] private GameObject articleTagline;
	[SerializeField] private GameObject articleSubject;
	[SerializeField] private GameObject articleImage;
	
	public void OnEnable()
	{
		if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
			setNewsPaper();
        }
	}
	
	public void setNewsPaper()
	{
		if(currentSession.currentDay <= 9)
		{
			newsPaperName.GetComponent<TMP_Text>().text = "Crasis Tribune";
			newsPaperTagline.GetComponent<TMP_Text>().text = "Corruption Withers in Ink";
		}
		if(currentSession.currentDay <= 6)
		{
			newsPaperName.GetComponent<TMP_Text>().text = "Melligo Herald";
			newsPaperTagline.GetComponent<TMP_Text>().text = "Yesterday's News Tomorrow";
		}
		if(currentSession.currentDay <= 3)
		{
			newsPaperName.GetComponent<TMP_Text>().text = "Anathor Gazette";
			newsPaperTagline.GetComponent<TMP_Text>().text = "Your Daily Disaster Bulletin";
		}
		string todayTitle = "No Title";
		string todayTagline = "No Tagline";
		string todayArticle = "No article";
		Sprite todayImage = null;
		int paperNumber = currentSession.currentDay - 1;
		if(paperNumber == 0)
		{
			bool loopCheck = false;
			for(int i = 0; i < 5; i++)
			{
				if(currentSession.endings[i] == true)
				{
					loopCheck = true;
					break;
				}
			}
			if(loopCheck == true)
			{
				todayTitle = NewsPapers[paperNumber].altTitle;
				todayTagline = NewsPapers[paperNumber].altTagline;
				todayArticle = NewsPapers[paperNumber].altPaper;
				todayImage = NewsPapers[paperNumber].altImage;
			}
			else
			{
				todayTitle = NewsPapers[paperNumber].standardTitle;
				todayTagline = NewsPapers[paperNumber].standardTagline;
				todayArticle = NewsPapers[paperNumber].standardPaper;
				todayImage = NewsPapers[paperNumber].standardImage;
			}
		}
		else
		{
			if(currentSession.levelBuildChoices[paperNumber - 1] == 1)
			{
				todayTitle = NewsPapers[paperNumber].standardTitle;
				todayTagline = NewsPapers[paperNumber].standardTagline;
				todayArticle = NewsPapers[paperNumber].standardPaper;
				todayImage = NewsPapers[paperNumber].standardImage;
			}
			else
			{
				todayTitle = NewsPapers[paperNumber].altTitle;
				todayTagline = NewsPapers[paperNumber].altTagline;
				todayArticle = NewsPapers[paperNumber].altPaper;
				todayImage = NewsPapers[paperNumber].altImage;
			}
		}
		articleTitle.GetComponent<TMP_Text>().text = todayTitle;
		articleTagline.GetComponent<TMP_Text>().text = todayTagline;
		articleSubject.GetComponent<TMP_Text>().text = todayArticle;
		articleImage.GetComponent<Image>().sprite = todayImage;
	}
}
