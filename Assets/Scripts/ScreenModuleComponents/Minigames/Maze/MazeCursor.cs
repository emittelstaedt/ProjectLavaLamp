using UnityEngine;
using System.Collections;

public class MazeCursor : ScreenModuleCursor
{
    [SerializeField] private float idleTimeToBlink = 1.0f;
    [SerializeField] private float blinkInterval = 0.25f;
    [SerializeField] private float blinkDuration = 3.0f;
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private Renderer cursorRenderer;
    private Coroutine blinkCoroutine;

    protected override void Awake()
    {
        base.Awake();

        startPosition = transform.position;
        cursorRenderer = GetComponent<Renderer>();
    }

    private void Update()
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

    private bool ShouldCancelBlink()
    {
        return transform.localPosition != lastPosition || transform.localPosition == startPosition;
    }
    
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
