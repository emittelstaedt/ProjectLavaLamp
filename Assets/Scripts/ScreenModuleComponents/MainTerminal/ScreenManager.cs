using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] screenManagers;
    [SerializeField] private GameObject[] screenCovers;
    private IScreen[] screens;
    private int activeScreenIndex;

    private void Awake()
    {
        screens = new IScreen[screenManagers.Length];

        for (int i = 0; i < screenManagers.Length; i++)
        {
            screens[i] = screenManagers[i] as IScreen;

            SetCoverActive(i, true);
        }
    }

    public void NextScreen()
    {
        ChangeScreen((activeScreenIndex + 1) % screens.Length);
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ScreenTransition, 0.1f, transform.position);
    }

    public void StopTerminalInteraction()
    {
        DeactivateScreen(activeScreenIndex);
    }

    private void ActivateScreen(int index)
    {
        screens[index].ActivateScreen();
        SetCoverActive(index, false);
    }

    private void DeactivateScreen(int index)
    {
        screens[index].DeactivateScreen();
        SetCoverActive(index, true);
    }

    private void ChangeScreen(int newIndex)
    {
        DeactivateScreen(activeScreenIndex);
        activeScreenIndex = newIndex;
        ActivateScreen(activeScreenIndex);
    }

    private void SetCoverActive(int index, bool isActive)
    {
        if (screenCovers[index] != null)
        {
            screenCovers[index].SetActive(isActive);
        }
    }
}
