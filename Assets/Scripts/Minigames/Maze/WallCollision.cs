using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private MazeManager mazeManager;

    void Awake()
    {
        mazeManager = GetComponentInParent<MazeManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<CursorController>() != null)
        {
            mazeManager?.ResetPlayerToStart();
        } 
    }
}
