using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private static float sensitivity = 0.15f;
    private Transform mainCamera;
    private float xRotation;
    private InputAction lookAction;

    public static float Sensitivity
    {
        get => sensitivity;
        set => sensitivity = value;
    }

    private void Awake()
    {
        mainCamera = Camera.main.transform;
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
            xRotation = mainCamera.localRotation.eulerAngles.x;
            
            // Accounts for eulerAngles only returning positive numbers.
            if (xRotation > 90)
            {
                xRotation -= 360;
            }

            xRotation -= lookAction.ReadValue<Vector2>().y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Camera should be a child of the player to rotate properly.
            transform.Rotate(0f, lookAction.ReadValue<Vector2>().x * sensitivity, 0f);
            mainCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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
