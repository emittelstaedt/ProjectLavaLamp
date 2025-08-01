using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour
{
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private Camera screenCamera;
    [SerializeField] private float idleTimeToBlink = 1.0f;
    [SerializeField] private float blinkInterval = 0.25f;
    [SerializeField] private float blinkDuration = 3.0f;
    private IScreenClickable currentClickable;
    private float screenWidth;
    private float screenHeight;
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Renderer cursorRenderer;
    private Coroutine blinkCoroutine;

    public Vector2 Position => (Vector2)transform.localPosition;

    void Awake()
    {
        screenHeight = screenCamera.orthographicSize * 2f;
        screenWidth = screenHeight * screenCamera.aspect;

        startPosition = transform.position;
        cursorRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (transform.position != lastPosition)
        {
            ResetCursor();

            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
        }
        else if (blinkCoroutine == null && !ShouldCancelBlink())
        {
            blinkCoroutine = StartCoroutine(BlinkAndResetRoutine());
        }

        lastPosition = transform.position;
    }

    public void Move(Vector2 movement)
    {
        transform.localPosition += (Vector3) movement * speed * Time.deltaTime;
        
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Clamp(localPosition.x, -screenWidth / 2f, screenWidth / 2f);
        localPosition.y = Mathf.Clamp(localPosition.y, -screenHeight / 2f, screenHeight / 2f);
        transform.localPosition = localPosition;
    }

    public void Click()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, Vector2.zero);
        if (hit)
        {
            currentClickable = hit.transform.gameObject.GetComponent<IScreenClickable>();
            
            currentClickable?.Click();
        }
    }
    
    public void Unclick()
    {
        currentClickable?.Unclick();
    }

    private bool ShouldCancelBlink() => transform.position != lastPosition || transform.position == startPosition;

    private IEnumerator BlinkAndResetRoutine()
    {
        float timer = 0f;

        while (timer < idleTimeToBlink)
        {
            timer += Time.deltaTime;

            if (ShouldCancelBlink())
            {
                yield break;
            }
            yield return null;
        }

        float blinkTimeElapsed = 0f;
        bool visible = true;

        while (blinkTimeElapsed < blinkDuration)
        {
            visible = !visible;
            SetCursorVisible(visible);

            yield return new WaitForSeconds(blinkInterval);
            blinkTimeElapsed += blinkInterval;

            if (ShouldCancelBlink())
            {
                yield break;
            }
        }

        transform.position = startPosition;
        ResetCursor();
    }

    private void SetCursorVisible(bool visible)
    {
        cursorRenderer.enabled = visible;
    }

    private void ResetCursor()
    {
        SetCursorVisible(true);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }
}
