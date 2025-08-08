using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    [SerializeField] private MinigameStatsSO stats;
    private float startTime;
    private float endTime;
    
    protected bool hasStarted;

    public MinigameStatsSO Stats => stats;

    public bool HasStarted => hasStarted;

    public void StartMinigame()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;
        startTime = Time.time;
        OnStartMinigame();
    }

    public void StopMinigame()
    {
        if (!hasStarted)
        {
            return;
        }

        hasStarted = false;
        endTime = Time.time;
        OnStopMinigame();
    }

    public float GetDuration() => endTime - startTime;

    public abstract void OnStartMinigame();
    public abstract void OnStopMinigame();
    public abstract void ResetMinigame();
}
