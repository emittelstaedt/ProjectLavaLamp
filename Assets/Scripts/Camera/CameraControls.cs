using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private float sensitivity = .15f;
    [SerializeField] private Transform mainCamera;
    private float xRotation;
    private InputAction lookAction;

    private void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");
    }

    public void Update()
    {
        MovePlayerCamera();
        LockCursor();
    }

    private void MovePlayerCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Allows the x rotation of the camera to be set externally.
            xRotation = mainCamera.transform.localRotation.eulerAngles.x;
            
            // Accounts for eulerAngles only returning positive numbers.
            if (xRotation > 90)
            {
                xRotation -= 360;
            }

            xRotation -= lookAction.ReadValue<Vector2>().y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Camera should be a child of the player to rotate properly.
            //transform.parent.Rotate(0f, lookAction.ReadValue<Vector2>().x * sensitivity, 0f);
            //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(0f, lookAction.ReadValue<Vector2>().x * sensitivity, 0f);
            mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    private void LockCursor()
    {
        if (!lookAction.enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
