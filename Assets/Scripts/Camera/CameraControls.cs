using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.15f;
    private Transform mainCamera;
    private float xRotation;
    private InputAction lookAction;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        lookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", sensitivity);
        MovePlayerCamera();
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

    public void SetCursorVisibility(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = isVisible;
    }
}
