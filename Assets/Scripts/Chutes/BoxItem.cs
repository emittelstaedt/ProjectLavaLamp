using UnityEngine;

[System.Serializable]
public class BoxItem
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private int count = 1;

    public GameObject Object => gameObject;
    public int Count => count;
}
