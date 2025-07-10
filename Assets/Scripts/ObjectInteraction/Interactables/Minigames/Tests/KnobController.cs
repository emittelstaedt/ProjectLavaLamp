using UnityEngine;

public class KnobController : MonoBehaviour, IMinigameClickable
{
    [SerializeField] private CursorController cursor;
    private bool isTurning = false;
    private float lastCursorAngle;
    private float currentCursorAngle;
    
    void Update()
    {
        if(isTurning)
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
