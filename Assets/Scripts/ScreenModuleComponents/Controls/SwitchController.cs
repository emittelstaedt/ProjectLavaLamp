using UnityEngine;
using System.Collections;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO switchUp;
    [SerializeField] private VoidEventChannelSO switchDown;
    [SerializeField] private Transform switchTransform;
    [SerializeField] private float switchTime = 0.05f;
    private Vector3 upDirection;
    private Vector3 downDirection;
    private Vector3 targetDirection;
    private Vector3 currentVelocity;
    
    void Awake()
    {
        upDirection = -switchTransform.right;
        downDirection = switchTransform.forward;
        targetDirection = downDirection;
    }
    
    void OnMouseDown()
    {
        VoidEventChannelSO switchEvent;
        if (targetDirection == upDirection)
        {
            targetDirection = downDirection;
            switchEvent = switchDown;
        }
        else
        {
            targetDirection = upDirection;
            switchEvent = switchUp;
        }
        
        if (switchEvent != null)
        {
            switchEvent.RaiseEvent();
        }

        StopAllCoroutines();
        StartCoroutine(Move());
    }
    
    public void OnModuleInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void OnModuleStopInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private IEnumerator Move()
    {
        while (Vector3.Distance(switchTransform.forward, targetDirection) > 0.001f)
        {
            Vector3 forward = switchTransform.forward;
            Vector3 currentDirection = Vector3.SmoothDamp(forward, targetDirection, ref currentVelocity, switchTime);
            
            switchTransform.LookAt(switchTransform.position + currentDirection, switchTransform.up);
            yield return null;
        }
    }
}
