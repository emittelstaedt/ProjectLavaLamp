using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveStick;
    [SerializeField][Range(0.0f, 1.0f)] private float speed = 0.3f;
    private float maxTilt = 45f;
    private bool isModuleBeingInteractedWith = false;
    private Vector3 defaultForward;
    private Vector3 tipPosition;
    private Vector3 targetDirection;
    
    void Awake()
    {
        defaultForward = transform.forward;
        tipPosition = transform.GetChild(0).position;
    }
    
    void OnMouseUp()
    {
        if (isModuleBeingInteractedWith)
        {
            StartCoroutine(MoveToDefaultPosition());
        }
    }
    
    void OnMouseDrag()
    {
        if (isModuleBeingInteractedWith)
        {
            TrackMouse();
            UpdateRotation();
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
        
        StartCoroutine(MoveToDefaultPosition());
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
        mouseScreenPosition.z = Vector3.Dot(tipPosition - Camera.main.transform.position,
                                            Camera.main.transform.forward);
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
