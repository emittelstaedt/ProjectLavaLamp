using UnityEngine;

public class MazeCollision : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO mazeCollision;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MazeCursor>())
        {
            if (mazeCollision != null)
            {
                if (mazeCollision.name == "MazeCollision")
                {
                    AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameError, 1f, transform.position);
                }
                mazeCollision.RaiseEvent();
            }
        }
    }
}
