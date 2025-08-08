using UnityEngine;
[CreateAssetMenu(fileName = "BuildInstructionsSO", menuName = "Scriptable Objects/BuildInstructionsSO")]
public class BuildInstructionsSO : ScriptableObject
{
    [SerializeField] private Sprite[] pages;
    
    public Sprite[] Pages => pages;
}