using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EfficiencyMonitor : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO lostGame;
    private TMP_Text efficiencyDisplay;
    private bool lockOut;
	private int speed;
    
	void Awake()
    {
        lockOut = false;
		speed = -1;
        //Get our child TMP object so we can edit its display live
        efficiencyDisplay = GetComponentInChildren<TextMeshProUGUI>();
        efficiencyDisplay.text = LevelManager.Instance.currentSession.efficiency.ToString();
		StartCoroutine(effiencyCheck());
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
				yield return new WaitForSeconds(1f / speed);
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
				yield return new WaitForSeconds(-1f / speed);
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

	public void modifyEfficiencySpeed(int modification)
	{
		speed += modification;
	}
	
	public void modifyEfficiencyValue(int modification)
	{
		LevelManager.Instance.currentSession.efficiency += modification;
	}
	
    public void LockOut()
    {
        lockOut = true;
    }
}
