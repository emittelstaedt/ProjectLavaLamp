using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VentMover : MonoBehaviour
{
	private bool ventTriggered;
	private PistonMover ventPiston;
	private Collider ventTriggerBox;
	
    public void Awake()
    {
		ventTriggered = false;
        ventPiston = this.transform.root.gameObject.GetComponentInChildren<PistonMover>();
		ventTriggerBox = GetComponent<Collider>();
    }

	private void OnTriggerExit(Collider other)
    {
		if(other.transform.parent != null)
		{
			if(other.transform.parent.gameObject.name == "Vent Cover" && ventTriggered == false)
			{
				ventTriggered = true;
				ventTriggerBox.enabled = false;
				//StartCoroutine(delayPush());
			}
		}
    }
	
	private IEnumerator delayPush()
	{
		yield return new WaitForSeconds(0.5f);
		ventPiston.StartPush();
	}
}
