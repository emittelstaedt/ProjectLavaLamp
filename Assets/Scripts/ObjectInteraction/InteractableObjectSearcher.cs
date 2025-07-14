using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObjectSearcher : MonoBehaviour
{
    private Transform mainCamera;
    private IInteractable currentInteraction;
    private IInteractable lastObjectLookedAt;
    private IInteractable currentObjectLookedAt;
    private InputAction interactAction;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        bool isLookingAtNewObject = false;
        bool canInteract = false;

        Ray seekingRay = new(mainCamera.position, mainCamera.forward);
        bool wasInteractPressedThisFrame = interactAction.WasPressedThisFrame();

        if (Physics.Raycast(seekingRay, out RaycastHit rayCastHit, 100f))
        {
            Transform hitTransform = rayCastHit.transform;
            currentObjectLookedAt = hitTransform.GetComponent<IInteractable>();

            isLookingAtNewObject = lastObjectLookedAt != null && lastObjectLookedAt != currentObjectLookedAt;
            canInteract = currentObjectLookedAt != null &&
                          currentObjectLookedAt.CanInteract() &&
                          GetDistanceToInteractable(currentObjectLookedAt) <= currentObjectLookedAt.GetInteractDistance();
        }

        if (canInteract)
        {
            if (isLookingAtNewObject)
            {
                lastObjectLookedAt.StopHover();
            }

            // Toggle interact start/stop when looking at an object.
            if (wasInteractPressedThisFrame)
            {
                HandleInteraction(currentObjectLookedAt);
            }
            // Start hover on the object if it hasn't been interacted with.
            else if (currentObjectLookedAt != currentInteraction)
            {
                currentObjectLookedAt.StartHover();
            }

            lastObjectLookedAt = currentObjectLookedAt;
        }
        // Stop hovering when looking from an interactable object to something that can't be interacted with.
        else if (lastObjectLookedAt != null)
        {
            lastObjectLookedAt.StopHover();
            lastObjectLookedAt = null;
        }
        // Stop interaction if the interact button is pressed while looking away from the interactable.
        else if (currentInteraction != null && wasInteractPressedThisFrame)
        {
            ClearCurrentInteraction();
        }

        // Stop interacting if player is too far away.
        if (currentInteraction != null)
        {
            if (GetDistanceToInteractable(currentInteraction) > currentInteraction.GetInteractDistance())
            {
                ClearCurrentInteraction();
            }
        }
    }

    private void HandleInteraction(IInteractable newInteraction)
    {
        if (currentInteraction == newInteraction)
        {
            ClearCurrentInteraction();
        }
        else
        {
            newInteraction.StopHover();
            newInteraction.StartInteract();
            currentInteraction = newInteraction;
        }
    }

    public void ClearCurrentInteraction()
    {
        currentInteraction.StopInteract();
        currentInteraction = null;
    }

    private float GetDistanceToInteractable(IInteractable interactable)
    {
        Vector3 distanceToInteractable = mainCamera.position - interactable.GetPosition();
        return distanceToInteractable.magnitude;
    }
}