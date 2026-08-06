using UnityEngine;
using UnityEngine.UI;

public class achievementIcon : MonoBehaviour
{
	[SerializeField] private int index;
	[SerializeField] private bool isEnding;
	[SerializeField] private Sprite Incomplete;
	[SerializeField] private Sprite Complete;
    
	public void achievementCheck()
	{
		Image icon = GetComponent<Image>();
		if(isEnding == true)
		{
			if(LevelManager.Instance.currentSession.endings[index] == true)
			{
				icon.sprite = Complete;
			}
			else
			{
				icon.sprite = Incomplete;
			}
		}
		else{
			if(LevelManager.Instance.currentSession.achievements[index] == true)
			{
				icon.sprite = Complete;
			}
			else
			{
				icon.sprite = Incomplete;
			}
		}

	}
}
