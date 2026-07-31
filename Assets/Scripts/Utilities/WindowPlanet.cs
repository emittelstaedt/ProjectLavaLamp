using UnityEngine;

public class WindowPlanet : MonoBehaviour
{
	[SerializeField] private Texture2D[] health;
	[SerializeField] private int planetNumber;
	private int planetHealth;
	private Material planetMaterial;
	private bool isAlive;
	
	public void Awake()
	{
		planetMaterial = GetComponent<Renderer>().material;
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
					isAlive = true;
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
				}
				break;
			default:
				Debug.Log("Error setting window planet");
				break;
		}
		if(isAlive == false)
		{
			planetHealth = 0;
		}
		planetMaterial.mainTexture = health[planetHealth];
	}
}
