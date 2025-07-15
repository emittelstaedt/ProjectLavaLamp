using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float timeToMove;
    [SerializeField] private Vector3 moveDistance;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;
    private bool isClosing = false;
    private float moveRate;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveDistance;
        float totalDistance = moveDistance.magnitude;
        moveRate = totalDistance / timeToMove;
    }

    private void Update()
    {
        if (isClosing)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveRate * Time.deltaTime);

            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
            {
                isClosing = false;
            }
        }
        else if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveRate * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isOpening = false;
            }
        }
    }

    public void Move()
    {
        isClosing = false;
        isOpening = true;
    }

    public void MoveBack()
    {
        isOpening = false;
        isClosing = true;
    }
}