using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private VoidEventChannelSO mazeComplete;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CursorController>())
        {
            if (mazeComplete != null)
            {
                mazeComplete.RaiseEvent();
            }

            if (stopInteract != null)
            {
                stopInteract.RaiseEvent();
            }

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);
        }
    }
}