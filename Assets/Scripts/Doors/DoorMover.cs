using UnityEngine;
using System.Collections;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO openingDoor;
    [SerializeField] private VoidEventChannelSO closingDoor;
    [SerializeField] private bool unlocked;
    private bool hasEntered;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Held")) && !hasEntered && unlocked)
        {
            hasEntered = true;
            openingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorOpen, 1f, transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Held")))
        {
            hasEntered = false;
            closingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorClose, 1.4f, transform.position);
        }
    }

    public void enableMainDoor()
    {
        GameObject camera = GameObject.FindWithTag("DoorCamera");
        if (camera != null)
        {
            SecurityCameraFollow securityCam = camera.GetComponent<SecurityCameraFollow>();
            if (securityCam != null)
            {
                if (securityCam.getState() != 2)
                {
                    unlocked = true;
                }
            }
            else
            {
                unlocked = true;
            }
        }
        else
        {
            unlocked = true;
        }
    }

    public void trueUnlock(){
        unlocked=true;
    }
}