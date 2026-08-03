using UnityEngine;

public class DrillManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO startDrillInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
	[SerializeField] private VoidEventChannelSO drillStopHelper;
    [SerializeField] private Transform drill;
    [SerializeField] private GameObject offScreen;
    [SerializeField] private GameObject[] DrillPrefabs;
    [SerializeField] private Vector3 drillStartPosition;
    private int currentDrillIndex;

    private bool hasStartedDrill;

    [SerializeField] private SoundType onCrash;

    private GameObject currentItemHeld;
    [SerializeField] private GameObject terminal5;
    
    void Awake()
    {
        currentItemHeld = null;
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

        int oldIndex = currentDrillIndex;

        do
        {
            currentDrillIndex = Random.Range(0, DrillPrefabs.Length);
        }
        while (currentDrillIndex == oldIndex && DrillPrefabs.Length > 1);

        DrillPrefabs[currentDrillIndex].SetActive(true);
    }

    public void StopDrillInteraction()
    {
        if (!hasStartedDrill)
        {
            return;
        }

        hasStartedDrill = false;


        if(stopInteract != null&&currentItemHeld!=null&&currentItemHeld==terminal5)
        {
            stopInteract.RaiseEvent();
        }

        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);
        TurnScreenOff(true);     
        ResetDrillToStart();
        LoadNextDrill();
        DrillPrefabs[currentDrillIndex].SetActive(false);
        hasStartedDrill=false;
    }

    public void PlayCrashSound()
    {
        AudioManager.Instance.PlaySound(MixerType.SFX, onCrash, 0.7f, transform.position);
    }

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        //Debug.Log($"Holding {currentItemHeld}");
    }
    
}
