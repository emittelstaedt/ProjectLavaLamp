using UnityEngine;

public class PlanetManager : MonoBehaviour, IScreen
{
    [SerializeField] private Canvas planetCanvas;

    public bool isActive()
    {
        return true;
    }

    public void ActivateScreen()
    {
        SetScreenState(true);
    }

    public void DeactivateScreen()
    {
        SetScreenState(false);
    }

    private void SetScreenState(bool isActive)
    {
        if (planetCanvas != null)
        {
            planetCanvas.enabled = isActive;
        }
    }
}
