using UnityEngine;

public interface IInteractable
{
    float GetInteractDistance();
    Vector3 GetPosition();
    bool CanInteract();
    void StartInteract();
    void StopInteract();
    void StartHover();
    void StopHover();
}
