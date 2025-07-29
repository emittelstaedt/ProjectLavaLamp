using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveStick;
    [SerializeField] private Camera moduleCamera;
    [SerializeField] [Range(0f, 1f)] private float speed = 0.3f;
    [SerializeField] private Transform tipTransform;
    private readonly float maxTilt = 45f;
    private bool isBeingControlled;
    private Vector3 defaultForward;
    private Vector3 targetDirection;
    private float tipOffsetFromCamera;

    void Awake()
    {
        defaultForward = transform.forward;
        
        Vector3 difference = tipTransform.position - moduleCamera.transform.position;
        tipOffsetFromCamera = Vector3.Dot(difference, moduleCamera.transform.forward);
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
        if (moveStick != null)
        {
            Vector3 localDirection = transform.localRotation * Vector3.forward;
            Vector2 inputDirection = (new Vector2(localDirection.x, localDirection.z)).normalized;
            Vector2 input = inputDirection * (Vector3.Angle(transform.forward, defaultForward) / maxTilt);
            moveStick.RaiseEvent(input);
        }
    }
    
    private void UpdateRotation()
    {
        float deltaDistance = speed * Vector3.Angle(transform.forward, targetDirection) * Time.deltaTime;
        Vector3 direction = Vector3.MoveTowards(transform.forward, targetDirection, deltaDistance);
        transform.LookAt(transform.position + direction, transform.up);
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
        
        while ((transform.forward - targetDirection).magnitude > 0.001f)
        {
            UpdateRotation();
            yield return null;
        }
    }
}
