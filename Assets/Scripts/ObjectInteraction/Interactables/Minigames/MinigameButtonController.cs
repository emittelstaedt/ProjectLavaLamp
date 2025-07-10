using UnityEngine;
using System.Collections;

public class MinigameButtonController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO buttonDown;
    [SerializeField] private VoidEventChannelSO buttonUp;
    [SerializeField] private string pressFunction;
    [SerializeField] private string unpressFunction;
    [SerializeField] private ButtonMode mode = ButtonMode.Normal;
    [SerializeField] private float pressDistance = 0.1f;
    [SerializeField] private float pressSpeed = 0.5f;
    private Vector3 defaultPosition;
    private bool isMoving = false;
    private bool isUnpressQueued = false;
    
    private enum ButtonMode
    {
        Normal,
        Hold,
        Toggle
    }
    
    void Start()
    {
        defaultPosition = transform.localPosition;
    }
    
    void OnMouseDown()
    {
        if(!isMoving && (mode != ButtonMode.Toggle || mode == ButtonMode.Toggle &&
                                                    transform.localPosition == defaultPosition))
        {
            StartCoroutine(PressAnimation());
            
        }
        else if(!isMoving)
        {
            StartCoroutine(UnpressAnimation());
        }
    }
    
    void OnMouseUp()
    {
        if(!isMoving && mode == ButtonMode.Hold)
        {
            StartCoroutine(UnpressAnimation());
        }
        else if(mode == ButtonMode.Hold)
        {
            isUnpressQueued = true;
        }
    }
    
    private IEnumerator PressAnimation()
    {
        isMoving = true;
        
        if(buttonDown != null)
        {
            buttonDown.RaiseEvent();
        }
        
        while(transform.localPosition.y > defaultPosition.y - pressDistance)
        {
            transform.localPosition -= Vector3.up * Time.deltaTime * pressSpeed;
            yield return null;
        }

        if(mode == ButtonMode.Normal || isUnpressQueued)
        {
            StartCoroutine(UnpressAnimation());
        }
        else
        {
            isMoving = false;
        }
    }
    
    private IEnumerator UnpressAnimation()
    {
        isMoving = true;
        
        if(buttonUp != null)
        {
            buttonUp.RaiseEvent();
        }
        
        while(transform.localPosition.y < defaultPosition.y)
        {
            transform.localPosition += Vector3.up * Time.deltaTime * pressSpeed;
            yield return null;
        }
        
        transform.localPosition = defaultPosition;
        
        isUnpressQueued = false;
        isMoving = false;
    }
}
