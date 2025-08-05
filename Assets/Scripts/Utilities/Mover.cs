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

    public void SetCanMove(bool newCanMove) => canMove = newCanMove;

    private void Awake()
    {
        startPosition = transform.localPosition;
        endPosition = startPosition + moveDistance;
        targetPosition = startPosition;
        moveRate = moveDistance.magnitude / timeToMove;
    }

    private void Update()
    {
        if (canMove)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveRate * Time.deltaTime);
        }
    }

    public void Move()
    {
        targetPosition = endPosition;
    }

    public void MoveBack()
    {
        targetPosition = startPosition;
    }
}