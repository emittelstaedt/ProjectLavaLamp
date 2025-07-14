using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveStick;
    [SerializeField] private Camera moduleCamera;
    private float maxTilt = 45f;
    private float mouseOffset = 0.2f;
    private float speed = 0.3f;
    private Vector3 defaultForward;
    private Vector3 targetDirection;
    private bool isTrackingMouse = false;
    
    void Awake()
    {
        defaultForward = transform.forward;
    }

    void Update()
    {
        if (moduleCamera.enabled)
        {
            if (isTrackingMouse)
            {
                TrackMouse();
            }
            else
            {
                targetDirection = defaultForward;  
            }
            
            UpdateRotation();
            SendInput();
        }
    }
    
    void OnMouseDown()
    {
        isTrackingMouse = true;
    }
    
    void OnMouseUp()
    {
        isTrackingMouse = false;
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
        mouseScreenPosition.z = (transform.position - Camera.main.transform.position).magnitude - mouseOffset;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        Vector3 joystickDirection = (mouseWorldPosition - transform.position).normalized;
        Vector3 clampedJoystickDirection = Vector3.RotateTowards(defaultForward, joystickDirection, maxTilt * Mathf.Deg2Rad, 0f);
        targetDirection = clampedJoystickDirection;
;
    }
}
