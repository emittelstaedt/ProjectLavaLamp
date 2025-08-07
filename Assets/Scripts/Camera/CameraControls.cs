using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private VoidEventChannelSO clearCrosshair;
    [SerializeField] private VoidEventChannelSO defaultCrosshair;
    private Transform mainCamera;
    private float xRotation;
    private InputAction lookAction;
    CharacterController characterController;
    float yCameraOffset;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        lookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();
        yCameraOffset = (characterController.height / 2) - mainCamera.localPosition.y;
    }

    public void Update()
    {
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", sensitivity);
        MovePlayerCamera();

        Vector3 newLocalPosition = new
        (
            mainCamera.localPosition.x,
            (characterController.height / 2) - yCameraOffset,
            mainCamera.localPosition.z
        );
        mainCamera.localPosition = newLocalPosition;
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
            clearCrosshair.RaiseEvent();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            defaultCrosshair.RaiseEvent();
        }

        Cursor.visible = isVisible;
    }
}
