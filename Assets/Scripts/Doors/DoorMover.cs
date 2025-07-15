using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO openingDoor;
    [SerializeField] private VoidEventChannelSO closingDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            openingDoor.RaiseEvent();
        }
        else
        {
            Debug.Log(other.gameObject.layer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            closingDoor.RaiseEvent();
        }
        else
        {
            Debug.Log(other.gameObject.layer);
        }
    }
}
