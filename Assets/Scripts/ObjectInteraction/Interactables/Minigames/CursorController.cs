using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private IScreenClickable currentClickable;
    
    public Vector2 Position => (Vector2) transform.localPosition;

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
}
