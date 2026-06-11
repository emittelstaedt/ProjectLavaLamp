using System;
using UnityEngine;

[System.Serializable]
public class EmployeeData
{
	public string employeeName;
	public int employeeNumber;
	public int currentDay;
	public int buildCountHPC;
	public int buildCountRebel;
	public int coffeeLevel;
	public int coffeeDrank;
	public int coffeeThrown;
	public float efficiency;
	
	public EmployeeData(){
		employeeName = "";
		employeeNumber = 0;
		currentDay = 1;
		buildCountHPC = 0;
		buildCountRebel = 0;
		coffeeLevel = 3;
		coffeeDrank = 0;
		coffeeThrown = 0;
		efficiency = 1000f;
	}
}
