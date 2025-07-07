using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildItems : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField][Range(0f, 1f)] private float distancePercentageToDrop = 0.1f;
    [SerializeField] private float rotationSpeed = 100f;
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
    private Vector3[] localCorners = new Vector3[8];
    private bool isHeld = false;
    private InteractableObjectSearcher searcher;

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

            CheckCorners(cameraPosition);

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
            transform.SetPositionAndRotation(targetPosition, playerCameraTransform.rotation * objectRotation);
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

        if (TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            Bounds localBounds = meshRenderer.localBounds;

            Vector3 min = localBounds.min;
            Vector3 max = localBounds.max;

            localCorners = new Vector3[8]
            {
                new(min.x, min.y, min.z),
                new(min.x, min.y, max.z),
                new(min.x, max.y, min.z),
                new(min.x, max.y, max.z),
                new(max.x, min.y, min.z),
                new(max.x, min.y, max.z),
                new(max.x, max.y, min.z),
                new(max.x, max.y, max.z)
            };
        }

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
    private void CheckCorners(Vector3 cameraPosition)
    {
        for (int i = 0; i < localCorners.Length; i++)
        {
            Vector3 worldCorner = transform.TransformPoint(localCorners[i]);
            Vector3 direction = (worldCorner - cameraPosition).normalized;
            float rayLength = Vector3.Distance(cameraPosition, worldCorner);

            if (Physics.Raycast(cameraPosition, direction, out RaycastHit hit, rayLength))
            {
                if (hit.collider.gameObject != this.gameObject && !hit.collider.CompareTag("IgnoreItemCollision"))
                {
                    float percentage = hit.distance / rayLength;
                    float newDistance = currentDistance * percentage;

                    if (newDistance < closestAllowedDistance)
                    {
                        searcher = playerCameraTransform.gameObject.GetComponent<InteractableObjectSearcher>();
                        searcher.ClearCurrentInteraction();
                        break;
                    }
                    else
                    {
                        currentDistance = newDistance;
                    }
                }
            }
        }
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