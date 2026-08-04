using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactGenerator : MonoBehaviour
{
	private EmployeeData currentSession;
	[SerializeField] private entry[] entries;
	[SerializeField] private EmailEventChannelSO sendEmail;
    
	public void Awake()
    {
		if(LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
		{
			currentSession = LevelManager.Instance.currentSession;
		}
		StartCoroutine(sendDailyFacts());
    }
	
	public IEnumerator sendDailyFacts()
	{
		yield return new WaitForSeconds(1.25f);
		for(int i = 0; i < entries.Length; i++)
		{
			if(LevelManager.Instance.currentSession.currentDay == entries[i].day)
			{
				StartCoroutine(delayedSend(entries[i].fact));
			}
		}
	}
	
	private IEnumerator delayedSend(email newMail)
	{
		yield return new WaitForSeconds(0.1f);
		sendEmail.RaiseEvent(newMail);
	}
}
