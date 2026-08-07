using UnityEngine;

public class ElevatorJammer : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO shutIt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        shutIt.RaiseEvent();
    }
}
