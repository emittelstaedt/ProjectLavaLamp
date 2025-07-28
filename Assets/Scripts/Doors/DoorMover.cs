using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO openingDoor;
    [SerializeField] private VoidEventChannelSO closingDoor;
    private bool hasEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEntered)
        {
            hasEntered = true;
            openingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorOpen, 1f, transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasEntered = false;
            closingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorClose, 1.4f, transform.position);
        }
    }
}