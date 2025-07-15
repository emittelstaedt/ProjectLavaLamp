using UnityEngine;
using UnityEngine.InputSystem;

public class KnobController : MonoBehaviour
{
    [SerializeField] private FloatEventChannelSO turnKnob;
    [Tooltip("0 results in free rotation.")]
    [SerializeField][Range(0f, 360f)] private float maxTurnAngle = 270f;
    private float currentRotation = 0f;
    private bool isModuleBeingInteractedWith = false;
    private float rotationOffset;
    private Vector3 defaultDirection;
    private float lastMouseAngle;
    private float currentMouseAngle;    
    private Vector3 tipPosition;
    
    void Awake()
    {
        tipPosition = transform.GetChild(0).position;
        defaultDirection = transform.up;
        rotationOffset = transform.eulerAngles.z;
    }
    
    void OnMouseDown()
    {
        if (isModuleBeingInteractedWith)
        {
            lastMouseAngle = GetAngleToMouse();
        }
    }
    
    void OnMouseDrag()
    {
        if (isModuleBeingInteractedWith)
        {
            Turn();
            SendInput();
        }
    }
    
    public void OnModuleInteract()
    {
        isModuleBeingInteractedWith = true;
    }
    
    public void OnModuleStopInteract()
    {
        isModuleBeingInteractedWith = false;
    }
    
    private void Turn()
    {
        currentMouseAngle = GetAngleToMouse();
        float angleDifference = currentMouseAngle - lastMouseAngle;
        if (Mathf.Abs(angleDifference) > 180f)
        {
            angleDifference = -Mathf.Sign(angleDifference) * (360f - Mathf.Abs(angleDifference));
        }
        
        currentRotation += angleDifference;
        currentRotation = Mathf.Clamp(currentRotation, 0f, maxTurnAngle);
        
        Vector3 currentAngles = transform.eulerAngles;
        currentAngles.z = currentRotation + rotationOffset;
        transform.eulerAngles = currentAngles;

        lastMouseAngle = currentMouseAngle;
    }
    
    private void SendInput()
    {
        turnKnob?.RaiseEvent(currentRotation / maxTurnAngle);
    }

    private float GetAngleToMouse()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = Vector3.Dot(tipPosition - Camera.main.transform.position,
                                            Camera.main.transform.forward);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        float distanceToMousePlane = (tipPosition - transform.position).magnitude;
        mouseWorldPosition += Camera.main.transform.forward * distanceToMousePlane;
        
        return Vector3.SignedAngle(defaultDirection, mouseWorldPosition - transform.position, transform.forward);
    }
}
