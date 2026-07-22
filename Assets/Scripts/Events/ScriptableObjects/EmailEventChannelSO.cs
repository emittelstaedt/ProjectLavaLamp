using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries Email payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/Email Event Channel", fileName = "EmailEventChannel")]
public class EmailEventChannelSO : GenericEventChannelSO<email>
{
}
