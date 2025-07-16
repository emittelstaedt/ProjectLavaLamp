using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveStick;
    [SerializeField][Range(0.0f, 1.0f)] private float speed = 0.3f;
    private float maxTilt = 45f;
    private bool isBeingControlled = false;
    private Vector3 defaultForward;
    private Vector3 targetDirection;
    private float tipOffsetFromCamera;
    private float distanceToTip;
    
    void Awake()
    {
        defaultForward = transform.forward;
    }
    
    void OnMouseDown()
    {
        isBeingControlled = true;
        StartCoroutine(TakeUserInput());
    }
    
    void OnMouseUp()
    {
        isBeingControlled = false;
        StartCoroutine(MoveToDefaultPosition());
    }
    
    public void OnModuleInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        
        // Only calculated the first time the module is interacted with.
        if (Mathf.Approximately(tipOffsetFromCamera, 0f))
        {
            Vector3 tipPosition = transform.GetChild(0).position;
            tipOffsetFromCamera = Vector3.Dot(tipPosition - Camera.main.transform.position,
                                              Camera.main.transform.forward);
            distanceToTip = (tipPosition - transform.position).magnitude;
        }
    }
    
    public void OnModuleStopInteract()
    {
        isBeingControlled = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        StartCoroutine(MoveToDefaultPosition());
    }
    
    private IEnumerator TakeUserInput()
    {
        while (isBeingControlled)
        {
            TrackMouse();
            UpdateRotation();
            SendInput();
            yield return null;
        }
    }
    
    private void SendInput()
    {
        Vector2 inputDirection = ((Vector2) (transform.forward - defaultForward)).normalized;
        Vector2 input = inputDirection * (Vector3.Angle(transform.forward, defaultForward) / maxTilt);
        
        moveStick.RaiseEvent(input);
    }
    
    private void UpdateRotation()
    {
        float deltaDistance = speed * Vector3.Angle(transform.forward, targetDirection) * Time.deltaTime;
        transform.LookAt(transform.position + Vector3.MoveTowards(transform.forward, targetDirection, deltaDistance));
    }
    
    private void TrackMouse()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = tipOffsetFromCamera;
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        Vector3 clampedDirection = Vector3.RotateTowards(defaultForward, direction, maxTilt * Mathf.Deg2Rad, 0f);
        targetDirection = clampedDirection;
    }
    
    private IEnumerator MoveToDefaultPosition()
    {
        targetDirection = defaultForward;
        
        while ((transform.forward - targetDirection).magnitude > 0.001)
        {
            UpdateRotation();
            yield return null;
        }
    }
}
