using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EfficiencyMonitor : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO lostGame;
    [SerializeField] private TMP_Text efficiencyDisplay;
	[SerializeField] private TMP_Text timeDisplay;
	[SerializeField] private GameObject amDisplay;
	[SerializeField] private GameObject pmDisplay;
    private bool lockOut;
	private int speed;
    private bool dayBegun;
	[SerializeField] private int startTime;
	[SerializeField] private int endTime;
	
	void Awake()
    {
		dayBegun = false;
        lockOut = false;
		speed = -1;
        efficiencyDisplay.text = LevelManager.Instance.currentSession.efficiency.ToString();
		setClock();
    }
	
	public void beginDay()
	{
		if(dayBegun == false)
		{
			StartCoroutine(effiencyCheck());
			StartCoroutine(dayTimer());
			dayBegun = true;
		}
	}
	
	private IEnumerator effiencyCheck()
	{
		while(lockOut == false)
		{
			if(LevelManager.Instance.currentSession.efficiency <= 0)
			{
				lostGame.RaiseEvent();
				lockOut = true;
			}
			if(speed > 0)
			{
				yield return new WaitForSeconds(1f / (2f * speed));
				if(LevelManager.Instance.currentSession.efficiency > 0)
				{
					LevelManager.Instance.currentSession.efficiency -= 1;
				}
				else
				{
					LevelManager.Instance.currentSession.efficiency = 0;
				}
			}
			if(speed < 0)
			{
				yield return new WaitForSeconds(-4f / speed);
				if(LevelManager.Instance.currentSession.efficiency < 1000)
				{
					LevelManager.Instance.currentSession.efficiency += 1;
				}
				else
				{
					LevelManager.Instance.currentSession.efficiency = 1000;
				}
			}
			else
			{
				yield return null;
			}
			efficiencyDisplay.text = LevelManager.Instance.currentSession.efficiency.ToString();
		}
	}

	public IEnumerator dayTimer()
	{
		while(lockOut == false)
		{
			yield return new WaitForSeconds(22.5f);
			startTime++;
			setClock();
			if(startTime == endTime && lockOut == false)
			{
				lostGame.RaiseEvent();
				lockOut = true;
			}
		}
	}
	
	public void setClock()
	{
		string hours = "00";
		string minutes = "00";
		switch(startTime / 4)
		{
			case 0:
				hours = "09";
				break;
			case 1:
				hours = "10";
				break;
			case 2:
				hours = "11";
				break;
			case 3:
				hours = "12";
				break;
			case 4:
				hours = "01";
				break;
			case 5:
				hours = "02";
				break;
			case 6:
				hours = "03";
				break;
			case 7:
				hours = "04";
				break;
			case 8:
				hours = "05";
				break;
			default:
				Debug.Log("error setting clock");
				break;
		}
		if(startTime % 4 == 0)
		{
			minutes = "00";
		}
		if(startTime % 4 == 1)
		{
			minutes = "15";
		}
		if(startTime % 4 == 2)
		{
			minutes = "30";
		}
		if(startTime % 4 == 3)
		{
			minutes = "45";
		}
		if(startTime >= 12)
		{
			amDisplay.SetActive(false);
			pmDisplay.SetActive(true);
		}
		else{
			amDisplay.SetActive(true);
			pmDisplay.SetActive(false);
		}
		string clockTime = hours + ":" + minutes;
		timeDisplay.text = clockTime;
	}
	
	public void modifyEfficiencySpeed(int modification)
	{
		speed += modification;
	}
	
	public void modifyEfficiencyValue(int modification)
	{
		if(LevelManager.Instance.currentSession.efficiency + modification > 1000)
		{
			LevelManager.Instance.currentSession.efficiency = 1000;
		}
		else
		{
			LevelManager.Instance.currentSession.efficiency += modification;
		}
	}
	
    public void LockOut()
    {
        lockOut = true;
    }
}
