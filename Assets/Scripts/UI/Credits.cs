using UnityEngine;

public class Credits : MonoBehaviour
{
	[SerializeField] private VoidEventChannelSO triggerMainMenu;
	
	public void PressMainMenuButton()
	{
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MenuClick, 0.5f);
		triggerMainMenu.RaiseEvent();
	}
}
