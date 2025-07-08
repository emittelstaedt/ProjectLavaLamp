using UnityEngine;
using UnityEngine.Events;

public abstract class GenericEventChannelSO<T> : ScriptableObject
{
    [Tooltip("The action to perform.")]
    public event UnityAction<T> EventRaised;

    public void OnEventRaised(T parameter)
    {
        EventRaised?.Invoke(parameter);
    }
}