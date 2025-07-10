using UnityEngine;

public class KnobController : MonoBehaviour, IMinigameClickable
{
    [SerializeField] private CursorController cursor;
    private bool isTurning = false;
    private float lastMouseAngle;
    private float currentMouseAngle;
    
    void Update()
    {
        if(isTurning)
        {
            Turn();
        }
    }

    public void Click()
    {
        lastMouseAngle = GetAngleToMouse();
        isTurning = true;
    }
    
    public void Unclick()
    {
        isTurning = false;
    }
    
    private void Turn()
    {
        currentMouseAngle = GetAngleToMouse();
        Vector3 newAngle = transform.eulerAngles;
        newAngle.z += currentMouseAngle - lastMouseAngle;
        lastMouseAngle = currentMouseAngle;
        transform.eulerAngles = newAngle;
    }
    
    private float GetAngleToMouse()
    {
        Vector2 direction = cursor.Position - (Vector2) transform.localPosition;
        return VectorToDegrees(direction);
    }
    
    private float VectorToDegrees(Vector2 direction)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
    }
}
