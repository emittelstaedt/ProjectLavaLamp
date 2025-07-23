using UnityEngine;

/// <summary>
/// PlayerTransform event channel that broadcasts and carries the player transform payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/PlayerTransformEventChannel", fileName = "PlayerTransformEventChannel")]
public class PlayerTransformEventChannelSO : GenericEventChannelSO<PlayerTransformPayload>
{
}