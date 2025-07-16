using UnityEngine;

public class SecurityCameraFollow : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;
    private readonly Quaternion rotationOffset = Quaternion.Euler(-90f, 180f, 0f);
    private Transform mainCameraTransform;

    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 directionToCamera = mainCameraTransform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up) * rotationOffset;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}