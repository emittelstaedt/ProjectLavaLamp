using UnityEngine;
using UnityEngine.InputSystem;

public class IInteractableSearcher : MonoBehaviour
{
    private IInteractable lastInteraction;
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
            
            if (wasInteractPressedThisFrame)
            {
                HandleInteraction(currentObjectLookedAt);
            }
            else if (currentObjectLookedAt != lastInteraction)
            {
                currentObjectLookedAt.StartHover();
            }

            lastObjectLookedAt = currentObjectLookedAt;
        }
        else if (lastObjectLookedAt != null)
        {
            lastObjectLookedAt.StopHover();
            lastObjectLookedAt = null;
        }
        else if (lastInteraction != null && wasInteractPressedThisFrame)
        {
            lastInteraction.StopInteract();
            lastInteraction = null;
        }
        
        // Stop interacting if player is too far away.
        if (lastInteraction != null)
        {
            Vector3 distanceToLastInteraction = (transform.position - lastInteraction.GetPosition());
            if (distanceToLastInteraction.magnitude > lastInteraction.GetInteractDistance())
            {
                lastInteraction.StopInteract();
                lastInteraction = null;
            }
        }
    }

    private void HandleInteraction(IInteractable newInteraction)
    {
        if (lastInteraction == newInteraction)
        {
            newInteraction.StopInteract();
            newInteraction.StopHover();
            lastInteraction = null;
        }
        else
        {
            if (lastInteraction != null)
            {
                lastInteraction.StopInteract();
                lastInteraction.StopHover();
            }

            newInteraction.StopHover();
            newInteraction.StartInteract();
            lastInteraction = newInteraction;
        }
    }
}
