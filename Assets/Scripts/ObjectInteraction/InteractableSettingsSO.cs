using UnityEngine;
[CreateAssetMenu(fileName = "InteractableSettingsSO", menuName = "Interaction/InteractableSettings", order = 0)]
public class InteractableSettingsSO : ScriptableObject
{
    [SerializeField, Range(0.5f, 10f)] private float interactionDistance = 3f;
    [SerializeField] private float outlineWidth = 5f;
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = Color.red;

    public float InteractionDistance => interactionDistance;
    public float OutlineWidth => outlineWidth;
    public Color HoverColor => hoverColor;
    public Color SelectedColor => selectedColor;
}