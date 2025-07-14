using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObjectSearcher : MonoBehaviour
{
    private Transform mainCamera;
    private IInteractable currentInteraction;
    private IInteractable lastObjectLookedAt;
    private InputAction interactAction;
    private readonly RaycastHit[] hits = new RaycastHit[100];

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        DetectInteractablesLookedAt();

        if (currentInteraction != null && !IsWithinRange(currentInteraction))
        {
             ClearCurrentInteraction();
        }
    }

    private void DetectInteractablesLookedAt()
    {
        List<IInteractable> currentObjectsLookedAt = new();
        Ray seekingRay = new(mainCamera.position, mainCamera.forward);

        int hitCount = Physics.RaycastNonAlloc(seekingRay, hits);

        // Sort hits by distance to prioritize closer interactables.
        Array.Sort(hits, 0, hitCount, Comparer<RaycastHit>.Create
        (
            (a, b) => a.distance.CompareTo(b.distance))
        );

        for (int i = 0; i < hitCount; i++) 
        {
            IInteractable interactable = hits[i].collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentObjectsLookedAt.Add(interactable);
            }
            else
            {
                // Stop adding objects if line of sight is blocked by a non-interactable object.
                break;
            }
        }

        HandleFoundInteractables(currentObjectsLookedAt);
    }

    private void HandleFoundInteractables(List<IInteractable> interactables)
    {
        UpdateHoverState(interactables);

        if (interactAction.WasPressedThisFrame())
        {
            TryInitiateInteraction(interactables);
        }
    }

    private void UpdateHoverState(List<IInteractable> interactables)
    {
        IInteractable newHoverTarget = null;

        foreach (IInteractable interactable in interactables)
        {
            if (CanInteractWith(interactable))
            {
                newHoverTarget = interactable;
                break;
            }
        }

        if (newHoverTarget != lastObjectLookedAt)
        {
            lastObjectLookedAt?.StopHover();
            newHoverTarget?.StartHover();
            lastObjectLookedAt = newHoverTarget;
        }
    }

    private void TryInitiateInteraction(List<IInteractable> interactables)
    {
        foreach (IInteractable interactable in interactables)
        {
            if (CanInteractWith(interactable))
            {
                ClearCurrentInteraction();
                
                interactable.StopHover();
                interactable.StartInteract();
                currentInteraction = interactable;

                return;
            }
        }

        if (currentInteraction != null)
        {
            ClearCurrentInteraction();
        }
    }

    public void ClearCurrentInteraction()
    {
        lastObjectLookedAt?.StopHover();
        lastObjectLookedAt = null;

        currentInteraction?.StopInteract();
        currentInteraction = null;
    }

    private bool CanInteractWith(IInteractable interactable)
    {
        return interactable.CanInteract() && IsWithinRange(interactable);
    }

    private bool IsWithinRange(IInteractable interactable)
    {
        return GetDistanceToInteractable(interactable) <= interactable.GetInteractDistance();
    }

    private float GetDistanceToInteractable(IInteractable interactable)
    {
        Vector3 distanceToInteractable = mainCamera.position - interactable.GetPosition();
        return distanceToInteractable.magnitude;
    }
}