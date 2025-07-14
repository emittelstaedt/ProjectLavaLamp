using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveStick;
    [SerializeField] private Camera moduleCamera;
    [SerializeField][Range(0.0f, 1.0f)] private float speed = 0.3f;
    private float maxTilt = 45f;
    private Vector3 defaultForward;
    private Vector3 tipPosition;
    private Vector3 targetDirection;
    private bool isTrackingMouse = false;
    
    void Awake()
    {
        defaultForward = transform.forward;
        tipPosition = transform.GetChild(0).position;
    }

    void Update()
    {
        if (IsModuleBeingInteractedWith())
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
    
    private bool IsModuleBeingInteractedWith()
    {
        return moduleCamera.enabled;
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
        mouseScreenPosition.z = Vector3.Dot(tipPosition - moduleCamera.transform.position,
                                            moduleCamera.transform.forward);
        Vector3 mouseWorldPosition = moduleCamera.ScreenToWorldPoint(mouseScreenPosition);
        
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        Vector3 clampedDirection = Vector3.RotateTowards(defaultForward, direction, maxTilt * Mathf.Deg2Rad, 0f);
        targetDirection = clampedDirection;
    }
}
