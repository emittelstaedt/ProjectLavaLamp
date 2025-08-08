using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO dropItem;
    [SerializeField] private GameObjectEventChannelSO heldItemChanged;
    [SerializeField] private GameObjectEventChannelSO heldItemCollision;
    [SerializeField] private VoidEventChannelSO defaultCrosshair;
    [SerializeField] private VoidEventChannelSO openHandCrosshair;
    [SerializeField] private VoidEventChannelSO closedHandCrosshair;
    [SerializeField] private StringEventChannelSO itemNameHover;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] [Range(0f, 1f)] private float distancePercentageToDrop = 0.1f;
    [SerializeField] private float rotationSpeed = 100f;
    private LayerMask ignoreCollisionLayer;
    private InputAction rotateAction;
    private InputAction lookAction;
    private Transform playerCameraTransform;
    private Collider[] itemColliders;
    private Rigidbody itemRigidbody;
    private float currentDistance;
    private Vector3 grabOffset;
    private float closestAllowedDistance;
    private Quaternion objectRotation;
    private bool isHeld;
    private GameObject currentItemHeld;
    private readonly Collider[] potentialHits = new Collider[10];

    private void Awake()
    {
        ignoreCollisionLayer = ~(1 << LayerMask.NameToLayer("IgnoreItemCollision"));

        rotateAction = InputSystem.actions.FindAction("Rotate");
        lookAction = InputSystem.actions.FindAction("Look");

        playerCameraTransform = Camera.main.transform;

        itemColliders = GetComponentsInChildren<Collider>();

        itemRigidbody = GetComponent<Rigidbody>();
    }

    // Uses late update to follow after camera controller script to prevent object stuttering.
    private void LateUpdate()
    {
        if (isHeld)
        {
            if (rotateAction.IsPressed())
            {
                lookAction.Disable();

                Vector2 mouseDelta = Input.mousePositionDelta * (rotationSpeed * Time.deltaTime);

                objectRotation = Quaternion.AngleAxis(mouseDelta.x, Vector3.down) * 
                                 Quaternion.AngleAxis(mouseDelta.y, Vector3.right) * objectRotation;
            }
            else
            {
                lookAction.Enable();
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
                    if (heldItemCollision != null)
                    {
                        heldItemCollision.RaiseEvent(gameObject);
                    }

                    break;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float soundVolume = Mathf.InverseLerp(0, 3, collision.relativeVelocity.magnitude);
        soundVolume *= 0.05f;

        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ItemDrop, soundVolume, transform.position);
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
        if (openHandCrosshair != null)
        {
            openHandCrosshair.RaiseEvent();
        }

        if (itemNameHover !=  null)
        {
            itemNameHover.RaiseEvent(this.name);
        }
    }

    public void StopHover()
    {
        if (isHeld && closedHandCrosshair != null)
        {
            closedHandCrosshair.RaiseEvent();
        }
        else if (defaultCrosshair != null)
        {
            defaultCrosshair.RaiseEvent();

            if (itemNameHover != null)
            {
                itemNameHover.RaiseEvent("");
            }
        }
    }

    public void SetSettings(VoidEventChannelSO dropItem, GameObjectEventChannelSO heldItemChanged, InteractableSettingsSO Settings)
    {
        this.dropItem = dropItem;
        this.heldItemChanged = heldItemChanged;
        this.Settings = Settings;
    }

    public void SetCrosshairChannels(VoidEventChannelSO defaultCrosshair, VoidEventChannelSO openHandCrosshair, 
                                     VoidEventChannelSO closedHandCrosshair, StringEventChannelSO itemNameHover)
    {
        this.defaultCrosshair = defaultCrosshair;
        this.openHandCrosshair = openHandCrosshair;
        this.closedHandCrosshair = closedHandCrosshair;
        this.itemNameHover = itemNameHover;
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

        float distanceToTarget = Vector3.Distance(cameraPosition, targetPosition);
        Vector3 directionToTarget = (targetPosition - cameraPosition).normalized;
        if (Physics.Raycast(cameraPosition, directionToTarget, out RaycastHit _, distanceToTarget, ignoreCollisionLayer))
        {
            // Object is behind something but not penetrating, so disallow movement.
            isValid = false;
        }

        collider.enabled = false;
        return isValid;
    }

    private void SetHeldState(bool isHeld)
    {
        if (isHeld && closedHandCrosshair != null)
        {
            closedHandCrosshair.RaiseEvent();
        }

        for (int i = 0; i < itemColliders.Length; i++)
        {
            itemColliders[i].enabled = !isHeld;
        }

        itemRigidbody.useGravity = !isHeld;
        itemRigidbody.linearVelocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        objectRotation = Quaternion.Inverse(playerCameraTransform.rotation) * transform.rotation;

        lookAction.Enable();

        this.isHeld = isHeld;
    }
}