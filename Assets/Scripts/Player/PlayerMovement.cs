using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Camera and player inputs
    [SerializeField] private Transform playerCamera;
    private CharacterController characterController;
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private float xRotation;

    //Physics variables
    private float speed = 5f;
    private float jumpForce = 10;
    private float sensitivity = 7;
    private float gravity = -9.81f;
    private Vector3 velocity;

    //Crouching
    private bool isCrouching = false;
    private float crouchSpeed = 10f;
    private float normalHeight = 2f;
    private float crouchHeight = 1f;

    //Sprinting
    private float sprintDuration = 2f;
    private float sprintTimer;
    private bool isSprinting = false;
    private float sprintCooldownDuration = 2f;
    private float sprintCooldownTimer;
    private bool isOnSprintCooldown = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        Sprint();
        MovePlayerCamera();
        Crouch();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(playerMovementInput);
        
        if (characterController.isGrounded)
        {
            velocity.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }


        characterController.Move(MoveVector * speed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            if (!isCrouching)
            {
                isSprinting = true;
                speed = 10f;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!isCrouching)
            {
                isSprinting = false;
                speed = 5f;
            }
        }
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if(sprintTimer <= 0)
            {
                isSprinting = false;
                speed = 5f;
                isOnSprintCooldown = true;
                sprintCooldownTimer = sprintCooldownDuration;
            }
        }

        if (isOnSprintCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0f)
            {
                isOnSprintCooldown = false;
            }
        }

        if(!isSprinting && !isOnSprintCooldown && sprintTimer < sprintDuration)
        {
            sprintTimer += Time.deltaTime;
            sprintTimer = Mathf.Min(sprintTimer, sprintDuration);
        }
    }

    private void MovePlayerCamera()
    {
        xRotation -= playerMouseInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(0f, playerMouseInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = true;
            speed = 3f;
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            isCrouching = false;
            speed = 5f;
        }

        if (isCrouching)
        {
            characterController.height = characterController.height - crouchSpeed * Time.deltaTime;
            if (characterController.height <= crouchHeight)
            {
                characterController.height = crouchHeight;
            }
        }
        else
        {
            characterController.height = characterController.height + crouchSpeed * Time.deltaTime;
            if(characterController.height >= normalHeight)
            {
                characterController.height = normalHeight;
            }
        }
    }
}