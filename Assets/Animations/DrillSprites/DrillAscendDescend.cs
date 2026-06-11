using UnityEngine;

public class DrillAscending : MonoBehaviour
{
    private Animator mAnimator;
    private float previousY;

    [SerializeField] private float threshold = 0.005f; 

    // Track the current movement state to prevent duplicate triggers
    private enum MovementState { None, Up, Down, Straight }
    private MovementState currentState = MovementState.None;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        previousY = transform.position.y;
        mAnimator.SetTrigger("StayingStraight");
    }

    void Update()
    {
        float currentY = transform.position.y;
        float deltaY = currentY - previousY;

        //Determine the target state for this frame
        MovementState targetState = MovementState.Straight;

        if (deltaY > threshold)
        {
            targetState = MovementState.Up;
        }
        else if (deltaY < -threshold)
        {
            targetState = MovementState.Down;
        }

        //Only fire the trigger if the state actually changed
        if (targetState != currentState)
        {
            currentState = targetState;

            switch (currentState)
            {
                case MovementState.Up:
                    mAnimator.SetTrigger("MovingUp");
                    break;
                case MovementState.Down:
                    mAnimator.SetTrigger("MovingDown");
                    break;
                case MovementState.Straight:
                    mAnimator.SetTrigger("StayingStraight");
                    break;
            }
        }

        previousY = currentY;
    }
}
