using UnityEngine;

public interface IScreen
{
    void ActivateScreen();
    void DeactivateScreen();
    bool isActive();
}