using UnityEngine;
using UnityEngine.InputSystem;

public class BuildItems : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float holdDistance = 3f;
    [SerializeField] private float closestAllowedDistance = 1.5f;
    [SerializeField] private float groundOffset = 0.25f;
    private Vector3[] localCorners = new Vector3[8];
    private Transform playerCameraTransform;
    private Outline outline;
    private Rigidbody itemRigidbody;
    private Collider itemCollider;
    private Quaternion rotationOffset;
    private InteractableObjectSearcher searcher;
    private InputAction rotateXAction;
    private InputAction rotateYAction;
    private float currentDistance;
    private bool isHeld = false;

    public float GetInteractDistance() => Settings.InteractionDistance;
    public Vector3 GetPosition() => transform.position;

    private void Awake()
    {
        rotateXAction = InputSystem.actions.FindAction("RotateX");
        rotateYAction = InputSystem.actions.FindAction("RotateY");

        playerCameraTransform = Camera.main.transform;

        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = Settings.OutlineWidth;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    private void LateUpdate()
    {
        if (isHeld)
        {
            currentDistance = holdDistance;
            Vector3 cameraPosition = playerCameraTransform.position;

            checkCorners(cameraPosition);

            if (rotateXAction.IsPressed())
            {
                rotationOffset *= Quaternion.Euler(Vector3.right * rotationSpeed * Time.deltaTime);
            }

            if (rotateYAction.IsPressed())
            {
                rotationOffset *= Quaternion.Euler(Vector3.up * rotationSpeed * Time.deltaTime);
            }

            Vector3 targetPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
            transform.position = targetPosition;
            transform.rotation = playerCameraTransform.rotation * rotationOffset;
        }
    }

    public void StartInteract()
    {
        currentDistance = holdDistance;

        // Finds all the corners of the object based on its local bounds
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Bounds localBounds = meshRenderer.localBounds;

            Vector3 min = localBounds.min;
            Vector3 max = localBounds.max;

            localCorners = new Vector3[]
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, max.y, max.z)
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

    // Creates raycasts to the object's corners to avoid clipping through things during transform movements
    private void checkCorners(Vector3 cameraPosition)
    {
        for (int i = 0; i < localCorners.Length; i++)
        {
            Vector3 worldCorner = transform.TransformPoint(localCorners[i]);
            Vector3 direction = (worldCorner - cameraPosition).normalized;

            if (Physics.Raycast(cameraPosition, direction, out RaycastHit hit, holdDistance + 0.1f))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag != "Ground" && hit.distance < closestAllowedDistance)
                    {
                        searcher = playerCameraTransform.gameObject.GetComponent<InteractableObjectSearcher>();
                        searcher.ClearCurrentInteraction();
                        break;
                    }
                    else
                    {
                        float safeDistance = Mathf.Max(closestAllowedDistance - groundOffset, hit.distance - 0.5f);
                        currentDistance = Mathf.Min(holdDistance, safeDistance);
                    }
                }
            }
        }
    }

    private void SetHeldState(bool isObjectHeld)
    {
        // Turns off object collider if held to avoid flickering when it hits other colliders
        itemCollider.enabled = !isObjectHeld;

        itemRigidbody.useGravity = !isObjectHeld;
        itemRigidbody.linearVelocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        if (isObjectHeld)
        {
            rotationOffset = Quaternion.Inverse(playerCameraTransform.rotation) * transform.rotation;
        }

        isHeld = isObjectHeld;
    }

}