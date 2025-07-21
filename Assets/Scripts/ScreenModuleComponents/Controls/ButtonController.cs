using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO buttonDown;
    [SerializeField] private VoidEventChannelSO buttonUp;
    [SerializeField] private ButtonMode mode = ButtonMode.Hold;
    [SerializeField] private float pressTime = 0.1f;
    private float pressDistance = 0.1f;
    private float upPositionY;
    private bool isMoving = false;
    private bool isUnpressQueued = false;
    
    private enum ButtonMode
    {
        SinglePress,
        Hold,
        Toggle
    }
    
    void Start()
    {
        upPositionY = transform.localPosition.y;
    }
    
    void OnMouseDown()
    {
        if (!isMoving)
        {
            if (mode != ButtonMode.Toggle ||
                mode == ButtonMode.Toggle && transform.localPosition.y == upPositionY)
            {
                Press();
            }
            else
            {
                Unpress();
            }
        }
    }
    
    void OnMouseUp()
    {
        // Check the layermask to see if the player is interacting with a module.
        if (gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && mode == ButtonMode.Hold)
        {
            if (!isMoving)
            {
                Unpress();
            }
            else
            {
                isUnpressQueued = true;
            }
        }
    }
    
    public void OnModuleInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void OnModuleStopInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        if (transform.localPosition.y != upPositionY && !isMoving)
        {
            Unpress();
        }
    }
    
    private void Press()
    {
        StartCoroutine(MoveButton(upPositionY, upPositionY - pressDistance));
    }
    
    private void Unpress()
    {
        StartCoroutine(MoveButton(upPositionY - pressDistance, upPositionY));
    }
    
    private IEnumerator MoveButton(float start, float end)
    {
        isMoving = true;
        
        float timer = 0f;
        
        if (start == upPositionY)
        {
            buttonDown?.RaiseEvent();
        }
        else
        {
            buttonUp?.RaiseEvent();
        }
        
        while (timer < pressTime)
        {
            timer += Time.deltaTime;
            Vector3 newPosition = transform.localPosition;
            newPosition.y = Mathf.Lerp(start, end, timer / pressTime);
            transform.localPosition = newPosition;
            yield return null;
        }

        if (start == upPositionY && (mode == ButtonMode.SinglePress || isUnpressQueued))
        {
            StartCoroutine(MoveButton(end, start));
            isUnpressQueued = false;
        }
        else
        {
            isMoving = false;
        }
    }
}
