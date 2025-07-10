using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private IMinigameClickable currentClickable;
    
    public Vector2 Position => (Vector2) transform.localPosition;

    public void Click()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, Vector2.zero);
        if(hit)
        {
            IMinigameClickable clickObject = hit.transform.gameObject.GetComponent<IMinigameClickable>();
            currentClickable = clickObject;
            
            if(clickObject != null)
            {
                clickObject.Click();
            }
        }
    }
    
    public void Unclick()
    {
        if(currentClickable != null)
        {
            currentClickable.Unclick();
        }
    }
}
