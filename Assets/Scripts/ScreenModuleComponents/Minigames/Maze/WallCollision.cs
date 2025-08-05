using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO mazeCollision;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mazeCollision != null)
        {
            mazeCollision.RaiseEvent();
        }
    }
}
