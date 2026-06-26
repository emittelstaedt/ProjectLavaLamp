    using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO buttonDown;
    [SerializeField] private VoidEventChannelSO buttonUp;
    [SerializeField] private Mode mode = Mode.Hold;
    [SerializeField] private float pressTime = 0.1f;
    private readonly float pressDistance = 0.1f;
    private Vector3 defaultLocalPosition;
    private float pressProgress;
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
        defaultLocalPosition = transform.localPosition;
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
        Vector3 pressDirection = transform.localRotation * Vector3.back;

        float targetDistance = isTargetStatePressed ? pressDistance : 0f;

        while(Mathf.Abs(pressProgress - targetDistance) > 0.001f)
        {
            pressProgress = Mathf.SmoothDamp(pressProgress, targetDistance, ref pressVelocity, pressTime);
            transform.localPosition = defaultLocalPosition + pressDirection * pressProgress;
            yield return null;
        }

        pressProgress = targetDistance;
        transform.localPosition = defaultLocalPosition + pressDirection * pressProgress;
        
    }
}
