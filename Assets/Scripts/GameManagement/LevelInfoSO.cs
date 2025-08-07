using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "GameManagement/LevelInfoSO")]
public class LevelInfoSO : ScriptableObject
{
    [SerializeField] private BoxItemsSO[] packages;

    public BoxItemsSO[] Packages => packages;
}
