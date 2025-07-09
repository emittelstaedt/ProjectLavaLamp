using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameClickManager : MonoBehaviour
{
    [SerializeField] private Transform screenTransform;
    [SerializeField] private Camera screenViewer;
    private Camera minigameCamera;
    private IMinigameClicker currentlyClicked;
    private Vector2 mousePosition;
    private bool isMouseOnScreen;
    
    public Vector2 MousePosition => mousePosition;

    void Start()
    {
        minigameCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // Player can only click when interacting with the screen.
        if(screenViewer.enabled)
        {
            mousePosition = ScreenToMinigamePoint(Mouse.current.position.ReadValue());

            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                ClickInMinigame();
            }

            CheckForUnclick();
        }
    }
    
    private void ClickInMinigame()
    {
        RaycastHit hit;
        if(Physics.Raycast((Vector3) mousePosition + transform.position, Vector3.forward, out hit))
        {
            IMinigameClicker clickObject = hit.transform.gameObject.GetComponent<IMinigameClicker>();
            currentlyClicked = clickObject;
            
            if(clickObject != null)
            {
                clickObject.Click();
            }
        }
    }
    
    private void CheckForUnclick()
    {
        if((Mouse.current.leftButton.wasReleasedThisFrame || !isMouseOnScreen) && currentlyClicked != null)
        {
            currentlyClicked.Unclick();
            currentlyClicked = null;
        }
    }
    
    private Vector2 ScreenToMinigamePoint(Vector2 screenPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        Vector3 uv;
        
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == screenTransform)
        {
            isMouseOnScreen = true;
            uv = hit.textureCoord;
        }
        else
        {
            isMouseOnScreen = false;
            uv = Vector3.zero;
        }
        
        return (Vector2) (minigameCamera.ViewportToWorldPoint(uv) - transform.position);
    }
}
