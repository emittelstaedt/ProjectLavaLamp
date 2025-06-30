using UnityEngine;

public class InteractableObjectSearcher : MonoBehaviour
{
    [SerializeField] private Transform viewpointCamera;
    [SerializeField] private PlayerInputs playerInputs;
    private InteractableObject lastInteraction;
    private InteractableObject currentInteraction;
    private RaycastHit rayCastHit;
    private bool wasInteractPressedLastFrame = false;

    void Update()
    {
        wasInteractPressedLastFrame = playerInputs.interactAction.WasPressedThisFrame();
        HandleRaycast();
    }

    //Checks if camera is looking at interactable object and turns off highlight if not or handles interaction if it is
    private void HandleRaycast()
    {
        Ray seekingRay = new Ray(viewpointCamera.position, viewpointCamera.forward);

        if (Physics.Raycast(seekingRay, out rayCastHit, 100f))
        {
            var hitTransform = rayCastHit.transform;
            var interactableObject = hitTransform.GetComponent<InteractableObject>();

            if (interactableObject != null && rayCastHit.distance <= interactableObject.Settings.InteractionDistance)
            {
                if (wasInteractPressedLastFrame)
                {
                    HandleInteraction(interactableObject);
                }
                else
                {
                    if (currentInteraction != null && currentInteraction != interactableObject && currentInteraction != lastInteraction)
                    {
                        currentInteraction.StopHighlight();
                    }

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
        //Avoids multiple objects being highlighted at once
        if (currentInteraction != null && currentInteraction != interactableObject && currentInteraction != lastInteraction)
        {
            currentInteraction.StopHighlight();
        }

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