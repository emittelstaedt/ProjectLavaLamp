using System;
using UnityEngine;

[System.Serializable]
public class EmployeeData
{
	public string employeeName;
	public int employeeNumber;
	public int currentDay;
	public float totalGameTime;
	public int[] levelBuildChoices; //0 Incomplete 1 HPC 2 CMS
	public float[] levelCompleteTimes;
	public int coffeeLevel;
	public int coffeeDrank;
	public int coffeeThrown;
	public int efficiency;
	public bool[] endings;
	
	public EmployeeData(){
		employeeName = "";
		employeeNumber = 0;
		currentDay = 1;
		totalGameTime = 0f;
		levelBuildChoices = new int[9];
		levelCompleteTimes = new float[9];
		for(int i = 0; i < 9; i++)
		{
			levelBuildChoices[i] = 0;
			levelCompleteTimes[i] = 0f;
		}
		coffeeLevel = 3;
		coffeeDrank = 0;
		efficiency = 500;
		endings = new bool[5];
		for(int i = 0; i < 5; i++)
		{
			endings[i] = false;
		}
	}
}
