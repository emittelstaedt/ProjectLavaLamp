using UnityEngine;

public class MazeCollision : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO mazeComplete;
    [SerializeField] private VoidEventChannelSO mazeCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mazeCollision != null)
        {
            mazeCollision.RaiseEvent();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MazeCursor>())
        {
            if (mazeComplete != null)
            {
                mazeComplete.RaiseEvent();
            }
        }
    }
}
