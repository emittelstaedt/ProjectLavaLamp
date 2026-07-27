using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementChoice : MonoBehaviour
{


	[SerializeField] private ChoiceEventChannelSO callOtherOption;
    [SerializeField] private VoidEventChannelSO notChosen;
	[SerializeField] private GameObject[] conditionalParts;
	private GameObject[] conditionalColliders;
	[SerializeField] private int choiceID;
	private choice thisOption;
	private GameObject otherOption;
	private bool chosen;
	
	public void Awake()
	{
		otherOption = null;
		chosen = false;
		thisOption.ID = choiceID;
		thisOption.option = gameObject;
		conditionalColliders = new GameObject[conditionalParts.Length];
		for(int i = 0; i < conditionalParts.Length; i++)
		{
			string removeThis = "(Clone)";
			string colliderName = conditionalParts[i].name + "Collider";
			colliderName = colliderName.Replace(removeThis, "");
			Transform collider = conditionalParts[i].transform.Find(colliderName);
			conditionalColliders[i] = collider.gameObject;
		}
		callOtherOption.RaiseEvent(thisOption);
	}
	
	public void locateOtherOption(choice recieved)
	{
		if(recieved.ID == choiceID)
		{
			if(otherOption == null)
			{
				//Debug.Log("Recieved sending back");
				otherOption = recieved.option;
				callOtherOption.RaiseEvent(thisOption);
			}
		}
	}
	
	//this function needs to be executed when a part is placed on this object???
	public void checkIfChosen()
	{
		if(chosen == false)
		{
			foreach(Transform child in transform)
			{
				for(int i = 0; i < conditionalColliders.Length; i++)
				{
					if(child.name == conditionalColliders[i].name)
					{
						chosen = true;
						notChosen.RaiseEvent();
						break;
					}
				}
				if(chosen == true)
				{
					break;
				}
			}
		}
	}
	
	public void destroyPlacementTriggers()
	{
		if(chosen == false)
		{
			//Debug.Log("I have been forsaken");
			foreach(Transform child in transform)
			{
				if(child.name.Contains("Collider") != true)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}
}
