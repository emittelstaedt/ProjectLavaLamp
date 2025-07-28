using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries int payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/Int Event Channel", fileName = "IntEventChannel")]
public class IntEventChannelSO : GenericEventChannelSO<int>
{
}