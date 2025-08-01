using UnityEngine;

public class MazeTerminalListener : MonoBehaviour
{
    [SerializeField] private MazeManager mazeManager;
    [SerializeField] private VoidEventChannelSO startModuleInteraction;

    private void OnEnable()
    {
        startModuleInteraction.OnEventRaised += OnTerminalInteract;
    }

    private void OnTerminalInteract()
    {
        mazeManager.ResetPlayerToStart();
        mazeManager.TurnOnScreen();
    }
}
