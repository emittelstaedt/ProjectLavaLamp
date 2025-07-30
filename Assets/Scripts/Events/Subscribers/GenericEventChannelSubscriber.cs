using UnityEngine;
using UnityEngine.Events;

public abstract class GenericEventChannelSubscriber<T, TChannel> : MonoBehaviour
    where TChannel : GenericEventChannelSO<T>
{
    [Tooltip("The signal to listen for.")]
    [SerializeField] protected TChannel eventChannel;
    [Space]
    [Tooltip("Response to receiving signal from Event Channel")]
    [SerializeField] protected UnityEvent<T> response;

    public void OnEventRaised(T value)
    {
        response?.Invoke(value);
    }

    public void SetChannelAndResponse(TChannel eventChannel, UnityEvent<T> response)
    {
        this.eventChannel = eventChannel;
        this.response = response;
    }

    protected void OnEnable()
    {
        if (eventChannel != null)
        {
            eventChannel.OnEventRaised += OnEventRaised;
        }        
    }

    protected void OnDisable()
    {
        if (eventChannel != null)
        {
            eventChannel.OnEventRaised -= OnEventRaised;
        }
    }
}