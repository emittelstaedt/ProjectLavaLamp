using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class ReadableMemo : MonoBehaviour
{
	private EmployeeData currentSession;
	[SerializeField] private memoOptions[] Memos;
	[SerializeField] private GameObject playerName;
	[SerializeField] private GameObject subject;
	[SerializeField] private GameObject memoBody;
	
	public void OnEnable()
	{
		if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
			setMemo();
        }
	}
	
	public void setMemo()
	{
		StartCoroutine(delay());
	}
	
	private IEnumerator delay()
	{
		yield return null;
		string profileName = currentSession.employeeName;
		playerName.GetComponent<TMP_Text>().text = profileName;
		string todaySubject = "No subject";
		string todayMemo = "No Memo";
		int memoNumber = currentSession.currentDay - 1;
		if(memoNumber == 0)
		{
			bool loopCheck = false;
			for(int i = 0; i < 5; i++)
			{
				if(currentSession.endings[i] == true)
				{
					loopCheck = true;
					break;
				}
			}
			if(loopCheck == true)
			{
				todayMemo = Memos[memoNumber].altMemo;
				todaySubject = Memos[memoNumber].altSubject;
			}
			else
			{
				todayMemo = Memos[memoNumber].standardMemo;
				todaySubject = Memos[memoNumber].standardSubject;
			}
		}
		else
		{
			if(currentSession.levelBuildChoices[memoNumber - 1] == 1)
			{
				todayMemo = Memos[memoNumber].standardMemo;
				todaySubject = Memos[memoNumber].standardSubject;
			}
			else
			{
				todayMemo = Memos[memoNumber].altMemo;
				todaySubject = Memos[memoNumber].altSubject;
			}
		}
		subject.GetComponent<TMP_Text>().text = todaySubject;
		memoBody.GetComponent<TMP_Text>().text = todayMemo;
	}
}
