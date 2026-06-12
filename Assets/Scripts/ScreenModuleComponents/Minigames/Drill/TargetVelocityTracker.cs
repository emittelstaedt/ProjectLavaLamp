using UnityEngine;

public class TargetVelocityTracker : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    private Vector3 lastPosition;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Calculate velocity based on frame-by-frame position changes
        Velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}
