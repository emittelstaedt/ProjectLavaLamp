using UnityEngine;

public class InteractableObjectSearcher : MonoBehaviour
{
    [SerializeField] private PlayerInputs playerInputs;
    private InteractableObject lastInteraction;
    private InteractableObject currentInteraction;
    private RaycastHit rayCastHit;
    private bool wasInteractPressedThisFrame = false;

    void Update()
    {
        wasInteractPressedThisFrame = playerInputs.interactAction.WasPressedThisFrame();
        HandleRaycast();
    }

    // Checks if camera is looking at interactable object and turns off highlight if not or handles interaction if it is
    private void HandleRaycast()
    {
        Ray seekingRay = new Ray(this.transform.position, this.transform.forward);

        if (Physics.Raycast(seekingRay, out rayCastHit, 100f))
        {
            Transform hitTransform = rayCastHit.transform;
            InteractableObject interactableObject = hitTransform.GetComponent<InteractableObject>();

            if (interactableObject != null && rayCastHit.distance <= interactableObject.Settings.InteractionDistance)
            {
                // Avoid multiple objects being highlighted at once
                if (currentInteraction != null && currentInteraction != interactableObject && currentInteraction != lastInteraction)
                {
                    currentInteraction.StopHighlight();
                }

                if (wasInteractPressedThisFrame)
                {
                    HandleInteraction(interactableObject);
                }
                else
                {
                    currentInteraction = interactableObject;

                    if (currentInteraction != lastInteraction)
                    {
                        currentInteraction.HandleHighlight();
                    }
                }

                return;
            }
        }

        if (currentInteraction != null && currentInteraction != lastInteraction)
        {
            currentInteraction.StopHighlight();
            currentInteraction = null;
        }
    }

    private void HandleInteraction(InteractableObject interactableObject)
    {
        currentInteraction = interactableObject;

        if (lastInteraction == currentInteraction)
        {
            currentInteraction.StopInteract();
            currentInteraction.StopHighlight();
            lastInteraction = null;
        }
        else
        {
            if (lastInteraction != null)
            {
                lastInteraction.StopInteract();
                lastInteraction.StopHighlight();
            }

            currentInteraction.StartInteract();
            lastInteraction = currentInteraction;
        }
    }
}