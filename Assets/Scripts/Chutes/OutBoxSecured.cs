using UnityEngine;

public class OutBoxSecured : MonoBehaviour
{
	[SerializeField] private VoidEventChannelSO winLevel;
	
	public void outBoxInOutChute()
	{
		int buildChoice = 0;
		if(this.gameObject.GetComponent<CMS>() != null)
		{
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.FirstCMSBuild);}
			buildChoice = 2;
		}
		else{
			buildChoice = 1;
		}
		LevelManager.Instance.currentSession.levelBuildChoices[LevelManager.Instance.currentSession.currentDay - 1] = buildChoice;
		winLevel.RaiseEvent();
	}
}
