using UnityEngine;

public class DrillManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO startDrillInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private Transform drill;
    [SerializeField] private GameObject offScreen;
    [SerializeField] private GameObject[] DrillPrefabs;
    [SerializeField] private Vector3 drillStartPosition;
    private int currentDrillIndex;
    private int previousDrillIndex;

    private bool hasStartedDrill;
    
    void Awake()
    {
        currentDrillIndex = Random.Range(0, DrillPrefabs.Length);
        //DrillPrefabs[currentDrillIndex].SetActive(true); //Old method of loading, caused things to start before terminal got touched

        TurnScreenOff(true);
    }

    public void StartDrillInteraction()
    {
        if (hasStartedDrill)
        {
            return;
        }
        //Only activates prefab once the player interacts with the terminal(?)
        DrillPrefabs[currentDrillIndex].SetActive(true);
        //ResetDrillToStart();

        hasStartedDrill = true;

        if (startDrillInteract != null)
        {
            startDrillInteract.RaiseEvent();
        }
    }

    public void TurnScreenOff(bool isActive)
    {
        offScreen.SetActive(isActive);
    }

   public void ResetDrillToStart()
    {
        drill.localPosition = drillStartPosition;
        DrillPrefabs[currentDrillIndex].SetActive(false);
        DrillPrefabs[currentDrillIndex].SetActive(true);
    }

    public void LoadNextDrill()
    {
        DrillPrefabs[currentDrillIndex].SetActive(false);

        // Pick a Drill that is different from the last.
        do
        {
            currentDrillIndex = Random.Range(0, DrillPrefabs.Length);
        } while (currentDrillIndex == previousDrillIndex && DrillPrefabs.Length > 1);

        previousDrillIndex = currentDrillIndex;

        DrillPrefabs[currentDrillIndex].SetActive(true);
        hasStartedDrill = false;
    }

    public void StopDrillInteraction()
    {
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);

        if (stopInteract != null)
        {
            stopInteract.RaiseEvent();
        }

        TurnScreenOff(true);    
        ResetDrillToStart();
        DrillPrefabs[currentDrillIndex].SetActive(false);
        LoadNextDrill();
    }
}
