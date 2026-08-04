using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageGenerator : MonoBehaviour
{
	private EmployeeData currentSession;
	[SerializeField] private emailOptions[] Emails;
	[SerializeField] private EmailEventChannelSO sendEmail;
	
    public void Awake()
    {
		if(LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
		{
			currentSession = LevelManager.Instance.currentSession;
			buildEmail(1);
			buildEmail(2);
		}
    }

	private void buildEmail(int emailType) //1 = hpc 2 = cms
	{
		string emailShorthand = "";
		string emailHandle = "";
		string emailSubject = "";
		string emailContent = "";
		int emailNumber = currentSession.currentDay - 1;
		if(emailNumber == 0) //day 1 will always have the same emails
		{
			if(emailType == 1)
			{
				emailShorthand = Emails[emailNumber].HPCshortHand;
				emailHandle = Emails[emailNumber].HPChandle;
				emailSubject = Emails[emailNumber].option1HPCResponseSubject;
				emailContent = Emails[emailNumber].option1HPCResponseContent;
			}
			else
			{
				emailShorthand = Emails[emailNumber].CMSshortHand;
				emailHandle = Emails[emailNumber].CMShandle;
				emailSubject = Emails[emailNumber].option1CMSResponseSubject;
				emailContent = Emails[emailNumber].option1CMSResponseContent;
			}
		}
		else
		{
			if(currentSession.levelBuildChoices[emailNumber - 1] == 1) //did the hpc build the previous day
			{
				if(emailType == 1)
				{
					emailShorthand = Emails[emailNumber].HPCshortHand;
					emailHandle = Emails[emailNumber].HPChandle;
					emailSubject = Emails[emailNumber].option1HPCResponseSubject;
					emailContent = Emails[emailNumber].option1HPCResponseContent;
				}
				else
				{
					emailShorthand = Emails[emailNumber].CMSshortHand;
					emailHandle = Emails[emailNumber].CMShandle;
					emailSubject = Emails[emailNumber].option1CMSResponseSubject;
					emailContent = Emails[emailNumber].option1CMSResponseContent;
				}
			}
			else //did the cms build the previous day
			{
				if(emailType == 1)
				{
					emailShorthand = Emails[emailNumber].HPCshortHand;
					emailHandle = Emails[emailNumber].HPChandle;
					emailSubject = Emails[emailNumber].option2HPCResponseSubject;
					emailContent = Emails[emailNumber].option2HPCResponseContent;
				}
				else
				{
					emailShorthand = Emails[emailNumber].CMSshortHand;
					emailHandle = Emails[emailNumber].CMShandle;
					emailSubject = Emails[emailNumber].option2CMSResponseSubject;
					emailContent = Emails[emailNumber].option2CMSResponseContent;
				}	
			}
		}
		email newMail;
		newMail.shortHand = emailShorthand;
		newMail.handle = emailHandle;
		newMail.responseSubject = emailSubject;
		newMail.responseContent = emailContent;
		StartCoroutine(delayedSend(newMail));
	}
	
	private IEnumerator delayedSend(email newMail)
	{
		yield return new WaitForSeconds(0.5f);
		sendEmail.RaiseEvent(newMail);
	}
}
