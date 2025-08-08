using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private List<Minigame> loadedMinigames;
    [SerializeField] private FloatEventChannelSO planetDamageChannel;
    private readonly Dictionary<Minigame, float> nextTriggerTimes = new();
    private readonly HashSet<Minigame> sirenTriggered = new();

    public static MinigameManager Instance = null;

    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<MinigameManager>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach (Minigame minigame in loadedMinigames)
        {
            float cooldown = minigame.Stats.CooldownTime;
            nextTriggerTimes[minigame] = Time.time + cooldown;
        }
    }

    void Update()
    {
        foreach (Minigame minigame in loadedMinigames)
        {
            if (!minigame.HasStarted && Time.time >= nextTriggerTimes[minigame] && !sirenTriggered.Contains(minigame))
            {
                if (minigame.Stats != null)
                {
                    minigame.Stats.SirenStartChannel.RaiseEvent();
                }
                sirenTriggered.Add(minigame);
            }
        }
    }

    public void TriggerMinigame(Minigame minigame)
    {
        minigame.StartMinigame();
        if (minigame.Stats != null)
        {
            minigame.Stats.SirenStopChannel.RaiseEvent();
        }
    }

    public void CompleteMinigame(Minigame minigame)
    {
        minigame.StopMinigame();

        float duration = minigame.GetDuration();
        float damage = minigame.Stats.CalculateDamage(duration);
        planetDamageChannel.RaiseEvent(damage);

        float cooldown = minigame.Stats.CooldownTime;
        nextTriggerTimes[minigame] = Time.time + cooldown;

        sirenTriggered.Remove(minigame);
    }

    public bool IsMinigameTriggered(Minigame minigame)
    {
        return sirenTriggered.Contains(minigame);
    }
}