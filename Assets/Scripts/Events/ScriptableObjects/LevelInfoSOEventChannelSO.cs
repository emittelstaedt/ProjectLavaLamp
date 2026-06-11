using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries int payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/LevelInfoSO Event Channel", fileName = "LevelInfoSOEventChannel")]
public class LevelInfoSOEventChannelSO : GenericEventChannelSO<LevelInfoSO>
{
}