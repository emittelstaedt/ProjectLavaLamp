using UnityEngine;

public class TestKnobController : MonoBehaviour, IScreenClickable
{
    [SerializeField] private CursorController cursor;
    private bool isTurning = false;
    private float lastCursorAngle;
    private float currentCursorAngle;
    
    void Update()
    {
        if (isTurning)
        {
            Turn();
        }
    }

    public void Click()
    {
        lastCursorAngle = GetAngleToCursor();
        isTurning = true;
    }
    
    public void Unclick()
    {
        isTurning = false;
    }
    
    public void SetXPosition(float newPosition)
    {
        Vector3 position = transform.localPosition;
        position.x = 2f * newPosition - 1f;
        transform.localPosition = position;
    }
    
    public void SetYPosition(float newPosition)
    {
        Vector3 position = transform.localPosition;
        position.y = 2f * newPosition - 1f;
        transform.localPosition = position;
    }
    
    private void Turn()
    {
        currentCursorAngle = GetAngleToCursor();
        Vector3 newAngle = transform.eulerAngles;
        newAngle.z += currentCursorAngle - lastCursorAngle;
        lastCursorAngle = currentCursorAngle;
        transform.eulerAngles = newAngle;
    }
    
    private float GetAngleToCursor()
    {
        Vector2 direction = cursor.Position - (Vector2) transform.localPosition;
        return VectorToDegrees(direction);
    }
    
    private float VectorToDegrees(Vector2 direction)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
    }
}
