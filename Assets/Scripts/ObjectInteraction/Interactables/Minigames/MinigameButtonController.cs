using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO buttonDown;
    [SerializeField] private VoidEventChannelSO buttonUp;
    [SerializeField] private ButtonMode mode = ButtonMode.SinglePress;
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
                StartCoroutine(PressAnimation(upPositionY, upPositionY - pressDistance));
            }
            else
            {
                StartCoroutine(PressAnimation(upPositionY - pressDistance, upPositionY));
            }
        }
    }
    
    void OnMouseUp()
    {
        if (mode == ButtonMode.Hold)
        {
            if (!isMoving)
            {
                StartCoroutine(PressAnimation(upPositionY - pressDistance, upPositionY));
            }
            else
            {
                isUnpressQueued = true;
            }
        }
    }
    
    private IEnumerator PressAnimation(float start, float end)
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
            StartCoroutine(PressAnimation(end, start));
            isUnpressQueued = false;
        }
        else
        {
            isMoving = false;
        }
        
    }
}
