using UnityEngine;

public class BillboardZAxisWithDelayAndFix : MonoBehaviour
{
    [SerializeField] private Transform playerCamera; // Assign the camera in the Inspector
    [SerializeField] private float rotationSpeed = 2f; // Lower = more delay (laggy)

    // Offset: -90° on X to fix upside-down, +180° on Y to fix backwards
    private readonly Quaternion rotationOffset = Quaternion.Euler(-90f, 180f, 0f);

    private void LateUpdate()
    {
        if (playerCamera == null) return;

        // Direction from object to camera
        Vector3 directionToCamera = playerCamera.position - transform.position;

        // Compute base rotation so object faces camera
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

        // Apply the rotation offset
        targetRotation *= rotationOffset;

        // Smooth delayed rotation (lag effect)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
