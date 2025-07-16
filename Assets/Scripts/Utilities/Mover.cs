using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float timeToMove;
    [SerializeField] private Vector3 moveDistance;
    [SerializeField] bool canMove = true;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;
    private float moveRate;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + moveDistance;
        targetPosition = startPosition;
        moveRate = moveDistance.magnitude / timeToMove;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveRate * Time.deltaTime);
    }

    public void Move()
    {
        if (canMove)
        {
            targetPosition = endPosition;
        }
        
    }

    public void MoveBack()
    {
        if (canMove)
        {
            targetPosition = startPosition;
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}