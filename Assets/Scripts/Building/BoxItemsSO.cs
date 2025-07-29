using UnityEngine;

[CreateAssetMenu(fileName = "BoxItemsSO", menuName = "Building/BoxItemSO")]
public class BoxItemsSO : ScriptableObject
{
    [SerializeField] private GameObject[] items;

    public GameObject[] Items => items;
}
