using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// General Event Channel that carries no extra data.
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "VoidEventChannel")]
public class VoidEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform.")]
    public event UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}