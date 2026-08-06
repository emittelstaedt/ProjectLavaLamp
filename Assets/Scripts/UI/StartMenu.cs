using UnityEngine;

public class StartMenu : MonoBehaviour
{
	[SerializeField] private GameObject profileMenu;
	[SerializeField] private GameObject optionsMenu;
	
	public void StartGame()
	{
        AudioManager.Instance.PlaySound(MixerType.UI, SoundType.MenuClick, 0.5f);
		profileMenu.SetActive(true);
	}
	
	public void LoadOptionsMenu()
    {
        AudioManager.Instance.PlaySound(MixerType.UI, SoundType.MenuClick, 0.5f);
        optionsMenu.SetActive(true);
    }
	
	public void QuitGame()
    {
        AudioManager.Instance.PlaySound(MixerType.UI, SoundType.MenuClick, 0.5f);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
