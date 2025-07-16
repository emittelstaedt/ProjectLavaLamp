using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO openingDoor;
    [SerializeField] private VoidEventChannelSO closingDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            openingDoor.RaiseEvent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closingDoor.RaiseEvent();
        }
    }
}
