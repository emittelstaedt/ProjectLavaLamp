using UnityEngine;

[CreateAssetMenu(fileName = "BoxItemsSO", menuName = "Building/BoxItemSO")]
public class BoxItemsSO : ScriptableObject
{
    [SerializeField] [Range(0f, 1f)] private float explosiveness = 0.5f;
    [SerializeField] private BoxItem[] items;

    public float Explosiveness => explosiveness;
    public BoxItem[] Items => items;
}
