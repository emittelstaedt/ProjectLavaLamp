using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO dropItem;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField][Range(0f, 1f)] private float distancePercentageToDrop = 0.1f;
    [SerializeField] private float rotationSpeed = 100f;
    private InputAction rotateXAction;
    private InputAction rotateYAction;
    private Transform playerCameraTransform;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;
    private Outline outline;
    private Vector3[] worldCorners = new Vector3[8];
    private float currentDistance;
    private Vector3 grabOffset;
    private float closestAllowedDistance;
    private Quaternion objectRotation;
    private bool isHeld = false;

    private void Awake()
    {
        rotateXAction = InputSystem.actions.FindAction("RotateX");
        rotateYAction = InputSystem.actions.FindAction("RotateY");

        playerCameraTransform = Camera.main.transform;

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = Settings.OutlineWidth;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    // Uses late update to follow after camera controller script to prevent object stuttering.
    private void LateUpdate()
    {
        if (isHeld)
        {
            Vector3 cameraPosition = playerCameraTransform.position;

            // Object must have a box collider to properly grab the local corners.
            if (itemCollider is BoxCollider box)
            {
                Vector3 center = box.center;
                Vector3 offsetFromCenter = box.size * 0.5f;

                Vector3[] boxCornerDirection = 
                {
                    new(-1, -1, -1),
                    new(-1, -1,  1),
                    new(-1,  1, -1),
                    new(-1,  1,  1),
                    new( 1, -1, -1),
                    new( 1, -1,  1),
                    new( 1,  1, -1),
                    new( 1,  1,  1)
                };
                
                for (int i = 0; i < worldCorners.Length; i++)
                {
                    Vector3 localCorner = center + Vector3.Scale(offsetFromCenter, boxCornerDirection[i]);
                    worldCorners[i] = transform.TransformPoint(localCorner);
                }
            }

            if (rotateXAction.IsPressed())
            {
                objectRotation *= Quaternion.Euler(Vector3.right * (rotationSpeed * Time.deltaTime));
            }

            if (rotateYAction.IsPressed())
            {
                objectRotation *= Quaternion.Euler(Vector3.up * (rotationSpeed * Time.deltaTime));
            }

            Vector3 targetPosition = playerCameraTransform.position +
                                     playerCameraTransform.rotation * grabOffset * currentDistance;

            Quaternion finalRotation = playerCameraTransform.rotation * objectRotation;

            bool isValidMove = PassesCornerCheck(cameraPosition, targetPosition, finalRotation);

            if (isValidMove)
            {
                targetPosition = playerCameraTransform.position + 
                                 playerCameraTransform.rotation * grabOffset * currentDistance;

                transform.SetPositionAndRotation(targetPosition, finalRotation);
            }
            else 
            {
                dropItem.RaiseEvent();
            }
        }
    }

    public float GetInteractDistance()
    {
        return Settings.InteractionDistance;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void StartInteract()
    {
        Vector3 cameraToObject = transform.position - playerCameraTransform.position;
        Vector3 relativePositionToCamera = Quaternion.Inverse(playerCameraTransform.rotation) * cameraToObject;

        currentDistance = relativePositionToCamera.magnitude;
        grabOffset = relativePositionToCamera.normalized;

        closestAllowedDistance = Mathf.Max(1f, currentDistance * (1f - distancePercentageToDrop));

        SetHeldState(true);
    }

    public void StopInteract()
    {
        SetHeldState(false);
    }

    public void StartHover()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }

    // Raycasts to the object's corners to avoid clipping through things.
    private bool PassesCornerCheck(Vector3 cameraPosition, Vector3 targetPosition, Quaternion objectRotation)
    {
        Quaternion deltaRotation = objectRotation * Quaternion.Inverse(transform.rotation);
        float temporaryDistance = currentDistance;

        for (int i = 0; i < worldCorners.Length; i++)
        {
            Vector3 positionOffset = worldCorners[i] - transform.position;
            Vector3 futureCorner = targetPosition + deltaRotation * positionOffset;

            Vector3 direction = (futureCorner - cameraPosition).normalized;
            float rayLength = Vector3.Distance(cameraPosition, futureCorner);

            if (Physics.Raycast(cameraPosition, direction, out RaycastHit hit, rayLength))
            {
                if (hit.collider.gameObject != this.gameObject && !hit.collider.CompareTag("IgnoreItemCollision"))
                {
                    float percentage = hit.distance / rayLength;
                    float newDistance = currentDistance * percentage;

                    if (newDistance < closestAllowedDistance)
                    {
                        return false;
                    }
                    else if (newDistance < temporaryDistance)
                    {
                        temporaryDistance = newDistance;
                    }
                }
            }
        }
        currentDistance = temporaryDistance;
        return true;
    }

    private void SetHeldState(bool isObjectHeld)
    {
        // Avoids colliding with player.
        itemCollider.enabled = !isObjectHeld;

        itemRigidbody.useGravity = !isObjectHeld;
        itemRigidbody.linearVelocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        objectRotation = Quaternion.Inverse(playerCameraTransform.rotation) * transform.rotation;

        isHeld = isObjectHeld;
    }
}