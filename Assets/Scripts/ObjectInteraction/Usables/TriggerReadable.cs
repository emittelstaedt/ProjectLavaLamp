using System.Collections;
using System.Collections.Generic;
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
		StartCoroutine(GetOverlays());
	}
	
	public void Update()
	{
		if(currentlyReading == true)
		{
			if(GameObject.FindWithTag("Held") == null){
				if(GameObject.FindWithTag("Lose") == null)
				{
					HUD.SetActive(true);
					setCursorVisibility.RaiseEvent(false);
					readable.SetActive(false);
					currentlyReading = false;
					InputSystem.actions.FindActionMap("Player").Enable();
				}
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
	
	private IEnumerator GetOverlays()
	{
		yield return new WaitForSeconds(0.1f);
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
}
