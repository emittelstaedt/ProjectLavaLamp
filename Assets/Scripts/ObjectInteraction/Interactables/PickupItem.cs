using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO dropItem;
    [SerializeField] private GameObjectEventChannelSO heldItemChanged;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField][Range(0f, 1f)] private float distancePercentageToDrop = 0.1f;
    [SerializeField] private float rotationSpeed = 100f;
    private LayerMask ignoreCollisionLayer;
    private InputAction rotateXAction;
    private InputAction rotateYAction;
    private Transform playerCameraTransform;
    private Collider[] itemColliders;
    private Rigidbody itemRigidbody;
    private Outline outline;
    private float currentDistance;
    private Vector3 grabOffset;
    private float closestAllowedDistance;
    private Quaternion objectRotation;
    private bool isHeld = false;
    private bool wasPlaced;
    private GameObject currentItemHeld;
    private readonly Collider[] potentialHits = new Collider[10];

    private void Awake()
    {
        ignoreCollisionLayer = ~(1 << LayerMask.NameToLayer("IgnoreItemCollision"));

        rotateXAction = InputSystem.actions.FindAction("RotateX");
        rotateYAction = InputSystem.actions.FindAction("RotateY");

        playerCameraTransform = Camera.main.transform;

        itemColliders = GetComponentsInChildren<Collider>();

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

            for (int i = 0; i < itemColliders.Length; i++)
            {
                Transform itemTransform = itemColliders[i].transform;

                if (itemTransform.TryGetComponent<PlacementTrigger>(out _))
                {
                    // Skip placement points so their colliders don't interfere with item movement.
                    continue;
                }

                // Calculate the position offset based on the collider's position relative to the base collider.
                Vector3 positionOffset = predictedPosition + 
                                         (itemTransform.position - itemColliders[0].transform.position);
                Quaternion rotationOffset = finalRotation * itemTransform.localRotation;

                if (IsValidPosition(cameraPosition, itemColliders[i], positionOffset, rotationOffset))
                {
                    // Can't use predictedPosition since currentDistance may have changed inside IsValidPosition.
                    Vector3 targetPosition = cameraPosition + (cameraRotation * grabOffset * currentDistance);

                    transform.SetPositionAndRotation(targetPosition, finalRotation);
                }
                else
                {
                    dropItem.RaiseEvent();

                    // Play a thud sound when object gets auto dropped by colliding with another collider.
                    AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ItemDrop, 0.05f, transform.position);

                    break;
                }
            }
        }
    }

    // If object hits the ground, play dropping sound.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ItemDrop, 0.05f, transform.position);
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

    public bool CanInteract()
    {
        return currentItemHeld == null;
    }

    public void StartInteract()
    {
        heldItemChanged.RaiseEvent(this.gameObject);

        Vector3 cameraToObject = transform.position - playerCameraTransform.position;
        Vector3 relativePositionToCamera = Quaternion.Inverse(playerCameraTransform.rotation) * cameraToObject;

        currentDistance = relativePositionToCamera.magnitude;
        grabOffset = relativePositionToCamera.normalized;

        closestAllowedDistance = currentDistance * (1f - distancePercentageToDrop);

        SetHeldState(true);
    }

    public void StopInteract()
    {
        heldItemChanged.RaiseEvent(null);
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

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
    }

    public void UpdateChildrenColliders()
    {
        itemColliders = GetComponentsInChildren<Collider>();
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
        Collider[] itemColliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in itemColliders)
        {
            collider.enabled = !isHeld;
        }

        itemRigidbody.useGravity = !isHeld;
        itemRigidbody.linearVelocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        objectRotation = Quaternion.Inverse(playerCameraTransform.rotation) * transform.rotation;

        this.isHeld = isHeld;
    }
}