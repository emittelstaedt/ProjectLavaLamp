using UnityEngine;

public class WindowPlanet : MonoBehaviour
{
	[SerializeField] private Texture2D[] health;
	[SerializeField] private int planetNumber;
	private int planetHealth;
	private Material planetMaterial;
	
	public void Awake()
	{
		planetMaterial = GetComponent<Renderer>().material;
	}

	public void checkPlanetHealth()
	{
		planetHealth = 1;
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
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[2] == 2)
				{
					planetHealth++;
				}
				break;
			case 1:
				if(LevelManager.Instance.currentSession.levelBuildChoices[3] == 2)
				{
					planetHealth++;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[4] == 1)
				{
					planetHealth++;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[5] == 2)
				{
					planetHealth++;
				}
				break;
			case 2:
				if(LevelManager.Instance.currentSession.levelBuildChoices[6] == 1)
				{
					planetHealth++;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[7] == 2)
				{
					planetHealth++;
				}
				if(LevelManager.Instance.currentSession.levelBuildChoices[8] == 2)
				{
					planetHealth++;
				}
				break;
			default:
				Debug.Log("Error setting window planet");
				break;
		}
		planetMaterial.mainTexture = health[planetHealth];
	}
}
