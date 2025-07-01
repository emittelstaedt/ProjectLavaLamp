using UnityEngine;
public class InteractableObject : MonoBehaviour
{
    private Outline outline;
    public InteractableSettingsSO Settings;

    public void Awake()
    {
        outline = GetComponent<Outline>();
    
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = Settings.OutlineWidth;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public void StartInteract()
    {
        outline.OutlineColor = Settings.SelectedColor;
        outline.enabled = true;
    }

    public void StopInteract()
    {
        // Empty for future interaction
    }

    public void HandleHighlight()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHighlight()
    {
        outline.enabled = false;
    }
}