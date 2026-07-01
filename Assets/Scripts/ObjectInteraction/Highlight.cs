using UnityEngine;

public class Highlight : MonoBehaviour
{
	private Outline outline;
	[SerializeField] private InteractableSettingsSO highlightSettings;
	
	public void Awake()
	{
		if(gameObject.GetComponent<Outline>() != null)
		{
			outline = gameObject.GetComponent<Outline>();
		}
		else
		{
			Debug.Log("Error fetching outline");
		}
	}
	
	public void highlight()
	{
        outline.OutlineWidth = highlightSettings.OutlineWidth;
		outline.OutlineColor = highlightSettings.HoverColor;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
		outline.enabled = true;
	}
	
	public void unhighlight()
	{
		outline.enabled = false;
	}
}
