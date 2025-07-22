using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KnobController : MonoBehaviour
{
    [SerializeField] private FloatEventChannelSO turnKnob;
    [Tooltip("0 results in free rotation.")]
    [SerializeField] [Range(0f, 360f)] private float maxTurnAngle = 0f;
    [SerializeField] private Transform tipTransform;
    private bool isBeingControlled;
    private float currentRotation;
    private float lastMouseAngle;
    private float currentMouseAngle;  
    private float rotationOffset;
    private Vector3 defaultDirection;
    private float tipOffsetFromCamera;
    private float distanceToTip;
    
    void Awake()
    {
        defaultDirection = transform.up;
        rotationOffset = transform.eulerAngles.z;
    }
    
    void OnMouseDown()
    {
        isBeingControlled = true;
        lastMouseAngle = GetAngleToMouse();
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
        if (Mathf.Approximately(tipOffsetFromCamera, 0f))
        {
            Vector3 difference = tipTransform.position - Camera.main.transform.position;
            tipOffsetFromCamera = Vector3.Dot(difference, Camera.main.transform.forward);
            
            distanceToTip = (tipTransform.position - transform.position).magnitude;
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
            Turn();
            SendInput();
            yield return null;
        }
    }
    
    private void Turn()
    {
        currentMouseAngle = GetAngleToMouse();
        float angleDifference = currentMouseAngle - lastMouseAngle;
        
        // Accounts for currentMouseAngle flipping between high positive and high negative.
        if (Mathf.Abs(angleDifference) > 180f)
        {
            angleDifference = Mathf.Sign(angleDifference) * (Mathf.Abs(angleDifference) - 360f);
        }
        
        currentRotation += angleDifference;
        
        if (!Mathf.Approximately(maxTurnAngle, 0f))
        {
            currentRotation = Mathf.Clamp(currentRotation, 0f, maxTurnAngle);
        }
        
        Vector3 currentAngles = transform.eulerAngles;
        currentAngles.z = currentRotation + rotationOffset;
        transform.eulerAngles = currentAngles;

        lastMouseAngle = currentMouseAngle;
    }
    
    private void SendInput()
    {
        // Sends a value between 0 and 1 when maxTurnAngle is not 0. Otherwise, send a value equal to 1 per rotation.
        if (turnKnob != null)
        {
            float divisor = Mathf.Approximately(maxTurnAngle, 0f) ? 360f : maxTurnAngle;
            turnKnob.RaiseEvent(currentRotation / divisor);
        }
    }

    // Returns an angle from -180 to 180.
    private float GetAngleToMouse()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = tipOffsetFromCamera;
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition += Camera.main.transform.forward * distanceToTip;
        
        return Vector3.SignedAngle(defaultDirection, mouseWorldPosition - transform.position, transform.forward);
    }
}
