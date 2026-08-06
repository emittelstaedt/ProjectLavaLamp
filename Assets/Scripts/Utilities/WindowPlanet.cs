using UnityEngine;

public class WindowPlanet : MonoBehaviour
{
	[SerializeField] private Texture[] health;
	[SerializeField] private int planetNumber;
	private int planetHealth;
	private Renderer planetRenderer;
	private bool isAlive;
	
	public void Awake()
	{
		planetRenderer = GetComponent<Renderer>();
	}

	public void checkPlanetHealth()
	{
		planetHealth = 1;
		isAlive = false;
		switch(planetNumber)
		{
			case 0:
				if(LevelManager.Instance.currentSession.levelBuildChoices[0] == 1)
				{
					planetHealth++;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[1] == 2)
				{
					planetHealth++;
					isAlive = true;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[2] == 2)
				{
					planetHealth++;
					isAlive = true;
					if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.Planet1GoodEnding);}
				}
				break;
			case 1:
				if(LevelManager.Instance.currentSession.levelBuildChoices[3] == 2)
				{
					planetHealth++;
					isAlive = true;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[4] == 1)
				{
					planetHealth++;
					isAlive = true;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[5] == 2)
				{
					planetHealth++;
					isAlive = true;
					if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.Planet2GoodEnding);}
				}
				break;
			case 2:
				if(LevelManager.Instance.currentSession.levelBuildChoices[6] == 1)
				{
					planetHealth++;
					isAlive = true;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[7] == 2)
				{
					planetHealth++;
					isAlive = true;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[8] == 2)
				{
					planetHealth++;
					isAlive = true;
					if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.Planet3GoodEnding);}
				}
				break;
			default:
				Debug.Log("Error setting window planet");
				break;
		}
		if(isAlive == false && LevelManager.Instance.currentSession.currentDay == 1)
		{
			planetHealth = 0;
		}
		planetRenderer.material.SetTexture("_Planet", health[planetHealth]);
	}
}
