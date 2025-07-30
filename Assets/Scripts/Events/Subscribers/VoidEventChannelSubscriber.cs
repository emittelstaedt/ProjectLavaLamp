using UnityEngine;
using UnityEngine.Events;

public class VoidEventChannelSubscriber : MonoBehaviour
{
    [Tooltip("The signal to listen for.")]
    [SerializeField] private VoidEventChannelSO eventChannel;
    [Space]
    [Tooltip("Response to receiving signal from Event Channel")]
    [SerializeField] private UnityEvent response;

    public void OnEventRaised()
    {
        response?.Invoke();
    }

    public void SetChannelAndResponse(VoidEventChannelSO eventChannel, UnityEvent response)
    {
        this.eventChannel = eventChannel;
        this.response = response;
    }

    private void OnEnable()
    {
        if (eventChannel != null)
        {
            eventChannel.OnEventRaised += OnEventRaised;
        }
    }

    private void OnDisable()
    {
        if (eventChannel != null)
        {
            eventChannel.OnEventRaised -= OnEventRaised;
        }
    }
}