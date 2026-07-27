using UnityEngine;
using System.Collections;

public class Disappear : MonoBehaviour
{
	private PickupItem itemPickup;
	
	public void Awake()
	{
		if(GetComponent<PickupItem>() != null)
		{
			itemPickup = GetComponent<PickupItem>();
		}
	}
	
	public void triggerDisappear()
	{
		if(itemPickup != null)
		{
			Destroy(itemPickup);
		}
		StartCoroutine(delayedDestruction());
	}
	
	private IEnumerator delayedDestruction()
	{
		yield return new WaitForSeconds(1.5f);
		Destroy(gameObject);
	}
}
