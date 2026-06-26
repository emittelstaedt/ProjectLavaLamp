using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "GameManagement/LevelInfoSO")]
public class LevelInfoSO : ScriptableObject
{
    [SerializeField] private BoxItemsSO[] packages;
	[SerializeField] private BoxItemsSO CMSpackage;
    [SerializeField] private string buildName;
    [SerializeField] private BuildInstructionsSO buildInstructions;

    public BoxItemsSO[] Packages => packages;
	public BoxItemsSO CMSPackage => CMSpackage;
    public string BuildName => buildName;
    public BuildInstructionsSO BuildInstructions => buildInstructions;
}
