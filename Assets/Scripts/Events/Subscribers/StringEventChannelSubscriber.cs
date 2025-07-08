using UnityEngine;
using UnityEngine.Events;

public class StringEventChannelSubscriber : MonoBehaviour
{
    [Tooltip("The signal to listen for.")]
    [SerializeField] private StringEventChannelSO eventChannel;
    [Space]
    [Tooltip("Response to receiving signal from Event Channel")]
    [SerializeField] private UnityEvent<string> response;

    public void OnEventRaised(string value)
    {
        response?.Invoke(value);
    }

    private void OnEnable()
    {
        if (eventChannel != null)
        {
            eventChannel.EventRaised += OnEventRaised;
        }        
    }

    private void OnDisable()
    {
        if (eventChannel != null)
        {
            eventChannel.EventRaised -= OnEventRaised;
        }
    }
}