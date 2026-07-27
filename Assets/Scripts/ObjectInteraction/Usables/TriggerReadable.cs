using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerReadable : MonoBehaviour, IUsable
{
	[SerializeField] private BoolEventChannelSO setCursorVisibility;
	[SerializeField] private string readableTag;
	private GameObject readable;
	private bool currentlyReading;
	private GameObject HUD;
	
	public void Awake()
	{
		currentlyReading = false;
		HUD = GameObject.Find("HUD");
		Transform[] objects = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i].tag == readableTag)
			{
				readable = objects[i].gameObject;
			}
		}
	}
	public void UseItem()
	{
		if(currentlyReading == false)
		{
			HUD.SetActive(false);
			setCursorVisibility.RaiseEvent(true);
			readable.SetActive(true);
			currentlyReading = true;
			InputSystem.actions.FindActionMap("Player").Disable();
			InputSystem.actions.FindAction("UseItem").Enable();
		}
		else
		{
			HUD.SetActive(true);
			setCursorVisibility.RaiseEvent(false);
			readable.SetActive(false);
			currentlyReading = false;
			InputSystem.actions.FindActionMap("Player").Enable();
		}
	}
}
