using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private MazeManager mazeManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CursorController>())
        {
            mazeManager.LoadNextMaze();
            mazeManager.TurnOffScreen();
            stopInteract?.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);
        }
    }
}