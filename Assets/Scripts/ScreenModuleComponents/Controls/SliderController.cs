using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SliderController : MonoBehaviour
{
    [SerializeField] private FloatEventChannelSO sliding;
    [SerializeField] [Range(0f, 1f)] private float startingValue = 0f;
    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;
    private bool isBeingControlled;
    private Vector2 screenTop;
    private Vector2 screenBottom;
    private float currentValue;
    private float mouseValueOffset;
    
    void Awake()
    {
        currentValue = startingValue;
        SetPosition(currentValue);
    }

    void OnMouseDown()
    {
        isBeingControlled = true;
        
        mouseValueOffset = GetMouseValue() - currentValue;
        
        StartCoroutine(TakeUserInput());
    }
    
    void OnMouseUp()
    {
        isBeingControlled = false;
    }
    
    public void OnModuleInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        
        // Only calculated the first time the module is interacted with.
        if (screenTop == Vector2.zero && screenBottom == Vector2.zero)
        {
            screenTop = Camera.main.WorldToScreenPoint(top.position);
            screenBottom = Camera.main.WorldToScreenPoint(bottom.position);
        }
    }
    
    public void OnModuleStopInteract()
    {
        isBeingControlled = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    
    private IEnumerator TakeUserInput()
    {
        while (isBeingControlled)
        {
            Slide();
            yield return null;
        }
    }
    
    private void Slide()
    {
        if (sliding != null)
        {
            currentValue = Mathf.Clamp(GetMouseValue() - mouseValueOffset, 0f, 1f);
            
            sliding.RaiseEvent(currentValue);
            SetPosition(currentValue);
        }
    }
    
    private float GetMouseValue()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float dotProduct = Vector2.Dot(screenTop - screenBottom, mousePosition - screenBottom);
        return dotProduct / (screenTop - screenBottom).sqrMagnitude;
    }
    
    private void SetPosition(float newValue)
    {
        transform.position = (top.position - bottom.position) * newValue + bottom.position;
    }
}
