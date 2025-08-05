using UnityEngine;

public class TestMinigameManager : MonoBehaviour
{
    [SerializeField] private ScreenModuleCursor cursor;
    
    public void Button1()
    {
        cursor.transform.localPosition += new Vector3(0f, -0.05f, 0f);
    }
    
    public void CursorClick()
    {
        cursor.Click();
    }
    
    public void CursorUnclick()
    {
        cursor.Unclick();
    }
}
