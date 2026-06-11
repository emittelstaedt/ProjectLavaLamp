using UnityEngine;
using TMPro;

public class ProfileButton : MonoBehaviour
{
	[SerializeField] private int profileNumber;
	[SerializeField] private IntEventChannelSO sendProfileNumber;
	
	public void emitProfileNumber(){
		sendProfileNumber.RaiseEvent(profileNumber);
	}
	
	public void changeProfileName(EmployeeData profileName){
		if(profileName.employeeNumber == profileNumber)
		{
			TMP_Text profileNameText = GetComponentInChildren<TMP_Text>();
			if(profileName.employeeName != ""){
				profileNameText.text = profileName.employeeName;
			}else{
				profileNameText.text = "Blank";
			}
		}
	}
}
