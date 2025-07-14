using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO dropItem;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField][Range(0f, 1f)] private float distancePercentageToDrop = 0.1f;
    [SerializeField] private float rotationSpeed = 100f;
    private LayerMask ignoreCollisionLayer;
    private InputAction rotateXAction;
    private InputAction rotateYAction;
    private Transform playerCameraTransform;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;
    private Outline outline;
    private float currentDistance;
    private Vector3 grabOffset;
    private float closestAllowedDistance;
    private Quaternion objectRotation;
    private bool isHeld = false;
    private readonly Collider[] potentialHits = new Collider[10];

    private void Awake()
    {
        ignoreCollisionLayer = ~(1 << LayerMask.NameToLayer("IgnoreItemCollision"));

        rotateXAction = InputSystem.actions.FindAction("RotateX");
        rotateYAction = InputSystem.actions.FindAction("RotateY");

        playerCameraTransform = Camera.main.transform;

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();

        if (!TryGetComponent<Outline>(out outline))
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
            if (rotateXAction.IsPressed())
            {
                objectRotation *= Quaternion.Euler(Vector3.right * (rotationSpeed * Time.deltaTime));
            }

            if (rotateYAction.IsPressed())
            {
                objectRotation *= Quaternion.Euler(Vector3.up * (rotationSpeed * Time.deltaTime));
            }

            playerCameraTransform.GetPositionAndRotation(out Vector3 cameraPosition, out Quaternion cameraRotation);
            Vector3 predictedPosition = cameraPosition + (cameraRotation * grabOffset * currentDistance);
            Quaternion finalRotation = cameraRotation * objectRotation;

            if (IsValidPosition(cameraPosition, itemCollider, predictedPosition, finalRotation))
            {
                // Can't use predictedPosition since currentDistance may have changed inside IsValidPosition.
                Vector3 targetPosition = cameraPosition + (cameraRotation * grabOffset * currentDistance);

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

        closestAllowedDistance = currentDistance * (1f - distancePercentageToDrop);

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

    private bool IsValidPosition(Vector3 cameraPosition, Collider collider, Vector3 targetPosition, Quaternion objectRotation)
    {
        bool isValid = true;

        // Must briefly enable collider to check for collisions.
        collider.enabled = true;
        Vector3 extents = collider.bounds.extents;

        int hitCount = Physics.OverlapSphereNonAlloc(targetPosition, extents.magnitude, potentialHits, ignoreCollisionLayer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider potentialHit = potentialHits[i];

            if (potentialHit.isTrigger)
            {
                continue;
            }

            bool doesPenetrate = Physics.ComputePenetration
            (
                collider, targetPosition, objectRotation,
                potentialHit, potentialHit.transform.position, potentialHit.transform.rotation,
                out Vector3 direction, out float distance
            );

            if (doesPenetrate)
            {
                Vector3 targetToHit = potentialHit.ClosestPoint(targetPosition) - targetPosition;
                Vector3 boundsEdgePoint = targetPosition + targetToHit - direction * distance;

                float rayLength = Vector3.Distance(cameraPosition, boundsEdgePoint);
                Vector3 rayDirection = (boundsEdgePoint - cameraPosition).normalized;
                if (Physics.Raycast(cameraPosition, rayDirection, out RaycastHit hit, rayLength, ignoreCollisionLayer))
                {
                    float percentage = hit.distance / rayLength;
                    float newDistance = currentDistance * percentage;

                    if (newDistance < closestAllowedDistance)
                    {
                        isValid = false;
                        break;
                    }
                    else
                    {
                        currentDistance = newDistance;
                    }
                }
            }
        }

        collider.enabled = false;
        return isValid;
    }

    private void SetHeldState(bool isHeld)
    {
        // Disable the colliders to prevent physics interactions while the object is held.
        itemCollider.enabled = !isHeld;

        itemRigidbody.useGravity = !isHeld;
        itemRigidbody.linearVelocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        objectRotation = Quaternion.Inverse(playerCameraTransform.rotation) * transform.rotation;

        this.isHeld = isHeld;
    }
}