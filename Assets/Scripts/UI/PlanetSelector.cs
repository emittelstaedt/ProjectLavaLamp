using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
	[SerializeField] private RenderTexture Athanor;
	[SerializeField] private RenderTexture Melligo;
	[SerializeField] private RenderTexture Crasis;
	Shader planetShader;
	
	public void Awake()
	{
		planetShader = GetComponent<Renderer>().material.shader;
		selectPlanet();
	}
	
	public void selectPlanet()
	{
		int planetID = Shader.PropertyToID("_Planet");
		if(LevelManager.Instance.currentSession.currentDay <= 9)
		{
			Shader.SetGlobalTexture(planetID, Crasis);
		}
		if(LevelManager.Instance.currentSession.currentDay <= 6)
		{
			Shader.SetGlobalTexture(planetID, Melligo);
		}
		if(LevelManager.Instance.currentSession.currentDay <= 3)
		{
			Shader.SetGlobalTexture(planetID, Athanor);
		}
	}
}
