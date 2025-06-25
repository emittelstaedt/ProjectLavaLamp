using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraControls : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private PlayerController player;
    [SerializeField] private float sensitivity = 7f;
    private float xRotation;
    

    public void MovePlayerCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            xRotation -= playerInputs.PlayerMouseInput.y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            player.transform.Rotate(0f, playerInputs.PlayerMouseInput.x * sensitivity, 0f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    public void LockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
