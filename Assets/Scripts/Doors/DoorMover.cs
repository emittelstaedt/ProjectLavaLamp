using UnityEngine;
using System.Collections;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO openingDoor;
    [SerializeField] private VoidEventChannelSO closingDoor;
    [SerializeField] private bool unlocked;
    [SerializeField] private SoundType unlockedSound; //Used for announcements, primarily
    [SerializeField] private SoundType lockedSound; //Used for announcements, primarily
    [SerializeField] private SoundType soundOfUnlocking; //Sound made when door unlocks
    [SerializeField] private float volume = 1f;
    private bool hasPlayedUnlockedSound = false;
    private bool hasPlayedlockedSound = false;
    private bool hasEntered;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Held")) && !hasEntered && unlocked)
        {
            hasEntered = true;
            openingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorOpen, 1f, transform.position);
            //Play announcement when the player approaches this unlocked door the first time
            //We only want these to trigger on the first day.
            if(!hasPlayedUnlockedSound&&LevelManager.Instance.currentSession.currentDay==1)
            {
                AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, unlockedSound, volume);
                hasPlayedUnlockedSound = true;
            }
        }
        else if(!unlocked&&!hasPlayedlockedSound)
        {
            AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, lockedSound, volume);
            hasPlayedlockedSound = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (((other.CompareTag("Player") || other.CompareTag("Held"))) && unlocked && hasEntered)
        {
            hasEntered = false;
            closingDoor.RaiseEvent();

            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.DoorClose, 1.4f, transform.position);
        }
    }

    public void enableMainDoor()
    {

        if(unlocked)
        {
            return;
        }

        GameObject camera = GameObject.FindWithTag("DoorCamera");
        if (camera != null)
        {
            SecurityCameraFollow securityCam = camera.GetComponent<SecurityCameraFollow>();
            if (securityCam != null)
            {
                if (securityCam.getState() != 2)
                {
                    AudioManager.Instance.PlaySound(MixerType.SFX, soundOfUnlocking, 1f, transform.position);
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
        AudioManager.Instance.PlaySound(MixerType.SFX, soundOfUnlocking, 1f, transform.position);
        unlocked=true;
    }
}