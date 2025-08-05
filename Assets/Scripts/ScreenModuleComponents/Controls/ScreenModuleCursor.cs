using UnityEngine;
using System.Collections;

public abstract class ScreenModuleCursor : MonoBehaviour
{
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private Camera screenCamera;
    private IScreenClickable currentClickable;
    private float screenWidth;
    private float screenHeight;

    public Vector2 Position => (Vector2)transform.localPosition;

    protected virtual void Awake()
    {
        screenHeight = screenCamera.orthographicSize * 2f;
        screenWidth = screenHeight * screenCamera.aspect;
    }

    public virtual void Move(Vector2 movement)
    {
        transform.localPosition += (Vector3) movement * speed * Time.deltaTime;
        
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Clamp(localPosition.x, -screenWidth / 2f, screenWidth / 2f);
        localPosition.y = Mathf.Clamp(localPosition.y, -screenHeight / 2f, screenHeight / 2f);
        transform.localPosition = localPosition;
    }

    public virtual void Click()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, Vector2.zero);
        if (hit)
        {
            currentClickable = hit.transform.gameObject.GetComponent<IScreenClickable>();
            
            currentClickable?.Click();
        }
    }
    
    public virtual void Unclick()
    {
        currentClickable?.Unclick();
    }
}
