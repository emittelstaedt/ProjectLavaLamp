using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries BuildInstructionsSO payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/BuildInstructionsSO Event Channel", fileName = "BuildInstructionsSOEventChannel")]
public class BuildInstructionsSOEventChannelSO : GenericEventChannelSO<BuildInstructionsSO>
{
}
