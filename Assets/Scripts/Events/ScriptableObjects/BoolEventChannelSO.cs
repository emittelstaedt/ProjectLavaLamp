using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries bool payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/Bool Event Channel", fileName = "BoolEventChannel")]
public class BoolEventChannelSO : GenericEventChannelSO<bool>
{
}
