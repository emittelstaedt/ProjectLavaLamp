using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
	private LevelManager levelManager;
	
	private void Awake()
    {
		levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		checkSelectedProfile();
	}
	
	public void checkSelectedProfile()
	{
		Button button = gameObject.GetComponentInChildren<Button>();
		if(levelManager.currentSession.employeeName == ""){
			button.interactable = false;
		}else{
			button.interactable = true;
		}
	}
}
