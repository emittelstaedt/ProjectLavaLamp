using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "GameManagement/LevelInfoSO")]
public class LevelInfoSO : ScriptableObject
{
    [SerializeField] private BoxItemsSO[] packages;
    [SerializeField] private string buildName;
    [SerializeField] private BuildInstructionsSO buildInstructions;

    public BoxItemsSO[] Packages => packages;
    public string BuildName => buildName;
    public BuildInstructionsSO BuildInstructions => buildInstructions;
}
