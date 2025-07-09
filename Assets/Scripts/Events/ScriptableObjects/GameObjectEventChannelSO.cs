using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries GameObject payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/GameObject Event Channel", fileName = "GameObjectEventChannel")]
public class GameObjectEventChannelSO : GenericEventChannelSO<GameObject>
{
}