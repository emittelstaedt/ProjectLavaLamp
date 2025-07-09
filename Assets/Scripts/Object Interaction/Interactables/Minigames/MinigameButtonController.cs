using UnityEngine;
using System.Collections;

public class MinigameButtonController : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Transform buttonTransform;
    [SerializeField] private string pressFunction;
    [SerializeField] private string unpressFunction;
    [SerializeField] private ButtonMode mode = ButtonMode.Normal;
    [SerializeField] private float pressDistance = 0.1f;
    [SerializeField] private float pressSpeed = 0.5f;
    private Vector3 defaultPosition;
    private bool moving = false;
    private bool unpress = false;
    
    private enum ButtonMode
    {
        Normal,
        Hold,
        Toggle
    }
    
    void Start()
    {
        defaultPosition = buttonTransform.localPosition;
    }
    
    void OnMouseDown()
    {
        if(!moving && (mode != ButtonMode.Toggle || mode == ButtonMode.Toggle &&
                                                    buttonTransform.localPosition == defaultPosition))
        {
            StartCoroutine(PressAnimation());
            
        }
        else if(!moving)
        {
            StartCoroutine(UnpressAnimation());
        }
    }
    
    void OnMouseUp()
    {
        if(!moving && mode == ButtonMode.Hold)
        {
            StartCoroutine(UnpressAnimation());
        }
        else if(mode == ButtonMode.Hold)
        {
            unpress = true;
        }
    }
    
    private IEnumerator PressAnimation()
    {
        moving = true;
        
        gameManager.SendMessage(pressFunction);
        
        while(buttonTransform.localPosition.y > defaultPosition.y - pressDistance)
        {
            buttonTransform.localPosition -= Vector3.up * Time.deltaTime * pressSpeed;
            yield return null;
        }

        if(mode == ButtonMode.Normal || unpress)
        {
            StartCoroutine(UnpressAnimation());
        }
        else
        {
            moving = false;
        }
    }
    
    private IEnumerator UnpressAnimation()
    {
        moving = true;
        
        gameManager.SendMessage(unpressFunction, SendMessageOptions.DontRequireReceiver);
        
        while(buttonTransform.localPosition.y < defaultPosition.y)
        {
            buttonTransform.localPosition += Vector3.up * Time.deltaTime * pressSpeed;
            yield return null;
        }
        
        buttonTransform.localPosition = defaultPosition;
        
        unpress = false;
        moving = false;
    }
}
