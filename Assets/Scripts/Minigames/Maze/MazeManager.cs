using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject offScreen;
    [SerializeField] private GameObject[] mazePrefabs;
    private Vector3 startPosition;
    private int currentMazeIndex;
    private int previousMazeIndex;

    void Awake()
    {
        startPosition = cursor.position;

        currentMazeIndex = Random.Range(0, mazePrefabs.Length);
        mazePrefabs[currentMazeIndex].SetActive(true);

        TurnOffScreen();
    }

    public void TurnOffScreen()
    {
        offScreen.SetActive(true);
    }

    public void TurnOnScreen()
    {
        offScreen.SetActive(false);
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
    }
}
