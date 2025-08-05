using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO startMazeInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject offScreen;
    [SerializeField] private GameObject[] mazePrefabs;
    private Vector3 startPosition;
    private int currentMazeIndex;
    private int previousMazeIndex;
    private bool hasStartedMaze;
    
    void Awake()
    {
        startPosition = cursor.position;

        currentMazeIndex = Random.Range(0, mazePrefabs.Length);
        mazePrefabs[currentMazeIndex].SetActive(true);

        TurnScreenOff(true);
    }

    public void StartMazeInteraction()
    {
        if (hasStartedMaze)
        {
            return;
        }

        hasStartedMaze = true;

        if (startMazeInteract != null)
        {
            startMazeInteract.RaiseEvent();
        }
    }

    public void TurnScreenOff(bool isActive)
    {
        offScreen.SetActive(isActive);
    }

    public void ResetPlayerToStart()
    {
        cursor.position = startPosition;
    }

    public void LoadNextMaze()
    {
        mazePrefabs[currentMazeIndex].SetActive(false);

        // Pick a maze that is different from the last.
        do
        {
            currentMazeIndex = Random.Range(0, mazePrefabs.Length);
        } while (currentMazeIndex == previousMazeIndex && mazePrefabs.Length > 1);

        previousMazeIndex = currentMazeIndex;

        mazePrefabs[currentMazeIndex].SetActive(true);
        hasStartedMaze = false;
    }

    public void StopMazeInteraction()
    {
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);

        if (stopInteract != null)
        {
            stopInteract.RaiseEvent();
        }

        TurnScreenOff(true);
        LoadNextMaze();
    }
}
