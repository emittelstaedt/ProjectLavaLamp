using UnityEngine;
public class InteractableObject : MonoBehaviour
{
    private Outline outline;
    public InteractableSettingsSO Settings;

    public void Awake()
    {
        if (outline == null)
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
    }

    public void StartInteract()
    {
        outline.OutlineColor = Settings.SelectedColor;
        outline.enabled = true;
    }

    public void StopInteract()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void HandleHighlight()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHighlight()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}