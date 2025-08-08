using UnityEngine;

[CreateAssetMenu(fileName = "MinigameStatsSO", menuName = "Scriptable Objects/MinigameStatsSO")]
public class MinigameStatsSO : ScriptableObject
{
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private float damage;
    [SerializeField] private VoidEventChannelSO sirenStartChannel;
    [SerializeField] private VoidEventChannelSO sirenStopChannel;

    public VoidEventChannelSO SirenStartChannel => sirenStartChannel;
    public VoidEventChannelSO SirenStopChannel => sirenStopChannel;
    public float CooldownTime => cooldownTime;
    public float CalculateDamage(float timeTaken)
    {
        return timeTaken * damage / 2;
    }
}