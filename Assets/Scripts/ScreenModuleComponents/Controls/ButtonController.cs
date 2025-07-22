    using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO buttonDown;
    [SerializeField] private VoidEventChannelSO buttonUp;
    [SerializeField] private Mode mode = Mode.Hold;
    [SerializeField] private float pressTime = 0.1f;
    private readonly float pressDistance = 0.1f;
    private float defaultYPosition;
    private float pressVelocity;
    private bool isTargetStatePressed;
    private Coroutine moveButton;
    
    private enum Mode
    {
        Hold,
        Toggle
    }
    
    void Awake()
    {
        defaultYPosition = transform.localPosition.y;
    }

    void OnMouseDown()
    {
        if (mode == Mode.Hold || !isTargetStatePressed)
        {
            SetPressed(true);
        }
        else
        {
            SetPressed(false);
        }
    }
    
    void OnMouseUp()
    {
        if (mode == Mode.Hold)
        {
            SetPressed(false);
        }
    }
    
    public void OnModuleInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void OnModuleStopInteract()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        SetPressed(false);
    }
    
    private void SetPressed(bool isTargetStatePressed)
    {
        if (this.isTargetStatePressed != isTargetStatePressed)
        {
            this.isTargetStatePressed = isTargetStatePressed;

            if (isTargetStatePressed && buttonDown != null)
            {
                buttonDown.RaiseEvent();
            }
            else if (!isTargetStatePressed && buttonUp != null)
            {
                buttonUp.RaiseEvent();
            }

            if (moveButton != null)
            {
                StopCoroutine(moveButton);
            }

            moveButton = StartCoroutine(MoveButton());
        }
    }
    
    private IEnumerator MoveButton()
    {
        float targetYPosition;
        if (!isTargetStatePressed)
        {
            targetYPosition = defaultYPosition;
        }
        else
        {
            targetYPosition = defaultYPosition - pressDistance;
        }
        
        while (Mathf.Abs(transform.localPosition.y - targetYPosition) > 0.001f)
        {
            Vector3 position = transform.localPosition;
            position.y = Mathf.SmoothDamp(position.y, targetYPosition, ref pressVelocity, pressTime);
            
            transform.localPosition = position;
            yield return null;
        }
    }
}
