using UnityEngine;
using UnityEngine.InputSystem;

public class IInteractableSearcher : MonoBehaviour
{
    private IInteractable currentInteraction;
    private IInteractable lastObjectLookedAt;
    private IInteractable currentObjectLookedAt;
    private InputAction interactAction;

    private void Awake()
    {
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

        RaycastHit rayCastHit;
        Ray seekingRay = new Ray(transform.position, transform.forward);
        bool wasInteractPressedThisFrame = interactAction.WasPressedThisFrame();

        if (Physics.Raycast(seekingRay, out rayCastHit, 100f))
        {
            Transform hitTransform = rayCastHit.transform;
            currentObjectLookedAt = hitTransform.GetComponent<IInteractable>();
            
            isLookingAtNewObject = lastObjectLookedAt != null && lastObjectLookedAt != currentObjectLookedAt;
            canInteract = currentObjectLookedAt != null &&
                          rayCastHit.distance <= currentObjectLookedAt.GetInteractDistance();
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
            // Start hovering when the interaction ends if the player is still looking at the object.
            else if (currentObjectLookedAt != currentInteraction)
            {
                currentObjectLookedAt.StartHover();
            }

            lastObjectLookedAt = currentObjectLookedAt;
        }
        // Stop hovering when looking away from an object.
        else if (lastObjectLookedAt != null)
        {
            lastObjectLookedAt.StopHover();
            lastObjectLookedAt = null;
        }
        // Stop interaction if the interact button is pressed while looking away from the interactable.
        else if (currentInteraction != null && wasInteractPressedThisFrame)
        {
            currentInteraction.StopInteract();
            currentInteraction = null;
        }
        
        // Stop interacting if player is too far away.
        if (currentInteraction != null)
        {
            Vector3 distanceToCurrentInteraction = (transform.position - currentInteraction.GetPosition());
            if (distanceToCurrentInteraction.magnitude > currentInteraction.GetInteractDistance())
            {
                currentInteraction.StopInteract();
                currentInteraction = null;
            }
        }
    }

    private void HandleInteraction(IInteractable newInteraction)
    {
        if (currentInteraction == newInteraction)
        {
            currentInteraction.StopInteract();
            currentInteraction.StopHover();
            currentInteraction = null;
        }
        else
        {
            // Stop the current interaction.
            if (currentInteraction != null)
            {
                currentInteraction.StopInteract();
                currentInteraction.StopHover();
            }

            // Start the new interaction.
            newInteraction.StopHover();
            newInteraction.StartInteract();
            currentInteraction = newInteraction;
        }
    }
}
