using UnityEngine;
using System.Collections;

public class UIMovement : MonoBehaviour
{
    [Header("Hover Settings")]
    public float moveAmount = 0.25f;       
    public float bounceOvershoot = 0.15f;  
    public float moveDuration = 0.2f;     
    public float bounceDuration = 0.15f;   

    [Header("Click Settings")]
    public float scaleFactor = 0.9f;        
    public float rotationAmount = 20f;      
    public float clickAnimDuration = 0.15f; 

    private Vector3 originalLocalPos;
    private Vector3 hoverLocalPos;
    private Vector3 overshootLocalPos;

    private Vector3 originalScale;
    private Quaternion originalRotation;

    private bool isAnimating = false;
    private bool isHovered = false;
    private bool isClicked = false;

    private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;

        
        hoverLocalPos = originalLocalPos + Vector3.up * moveAmount;
        overshootLocalPos = originalLocalPos + Vector3.up * (moveAmount + bounceOvershoot);
    }

    void OnMouseEnter()
    {
        isHovered = true;
        if (!isAnimating)
            StartCoroutine(MoveUpWithBounce());
    }

    void OnMouseExit()
    {
        isHovered = false;
        if (!isAnimating)
            StartCoroutine(MoveBack());
    }

    void OnMouseDown()
    {
        isClicked = true;
        StopAllCoroutines();
        StartCoroutine(ClickAnimate(originalScale, originalRotation, originalScale * scaleFactor, originalRotation * Quaternion.Euler(0, 0, rotationAmount), clickAnimDuration));
    }

    void OnMouseUp()
    {
        isClicked = false;
        StopAllCoroutines();
        StartCoroutine(ClickAnimate(transform.localScale, transform.localRotation, originalScale, originalRotation, clickAnimDuration));

        
        if (isHovered)
            StartCoroutine(MoveUpWithBounce());
        else
            StartCoroutine(MoveBack());
    }

    IEnumerator MoveUpWithBounce()
    {
        isAnimating = true;
        yield return StartCoroutine(AnimateLocalPosition(transform.localPosition, hoverLocalPos, moveDuration));
        yield return StartCoroutine(AnimateLocalPosition(hoverLocalPos, overshootLocalPos, bounceDuration));
        yield return StartCoroutine(AnimateLocalPosition(overshootLocalPos, hoverLocalPos, bounceDuration));
        isAnimating = false;
    }

    IEnumerator MoveBack()
    {
        isAnimating = true;
        yield return StartCoroutine(AnimateLocalPosition(transform.localPosition, originalLocalPos, moveDuration));
        isAnimating = false;
    }

    IEnumerator AnimateLocalPosition(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = easeCurve.Evaluate(elapsed / duration);
            transform.localPosition = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;
    }

    IEnumerator ClickAnimate(Vector3 startScale, Quaternion startRot, Vector3 endScale, Quaternion endRot, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = easeCurve.Evaluate(elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
        transform.localRotation = endRot;
    }
}
