using UnityEngine;
using UnityEngine.InputSystem; 

public class ScreenModule : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO startInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private PlayerTransformEventChannelSO movePlayer;
    [SerializeField] private Camera moduleCamera;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private float distanceFromCamera = 0.5f;
    private Quaternion cachedCameraRotation;
    private Outline outline;
    private Camera mainCamera;
    private PlayerController playerController;
    private MeshRenderer playerMesh;
    private GameObject currentItemHeld;
    private bool isBeingUsed;

    public void Awake()
    {
        mainCamera = Camera.main;
        playerController = mainCamera.transform.GetComponentInParent<PlayerController>();
        playerMesh = playerController.gameObject.GetComponent<MeshRenderer>();
    
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = Settings.OutlineWidth;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }
    
    public float GetInteractDistance()
    {
        return Settings.InteractionDistance;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool CanInteract()
    {
        return !isBeingUsed && currentItemHeld == null;
    }

    public void StartInteract()
    {
        isBeingUsed = true;
        InputSystem.actions.FindActionMap("Player").Disable();
        playerMesh.enabled = false;
        
        CameraSwapper.Instance.SwapCameras(mainCamera, moduleCamera, EnablePlayerInteract);
        PutPlayerInFrontOfScreen();
    }
    
    public void StopInteract()
    {
        stopInteract.RaiseEvent();
        InputSystem.actions.FindAction("Interact").Disable();
        playerMesh.enabled = true;
        
        CameraSwapper.Instance.SwapCameras(moduleCamera, mainCamera, EnablePlayerControls);
        isBeingUsed = false;
    }
    
    public void StartHover()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }
    
    public void StopHover()
    {
        outline.enabled = false;
    }

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
    }

    private void EnablePlayerInteract()
    {
        startInteract.RaiseEvent();
        InputSystem.actions.FindAction("Interact").Enable();
    }
    
    private void EnablePlayerControls()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
    }

    private void PutPlayerInFrontOfScreen()
    {
        Vector3 targetPosition = moduleCamera.transform.position - moduleCamera.transform.forward * distanceFromCamera;

        if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit))
        {
            Vector3 finalPosition = targetPosition + Vector3.down * hit.distance;

            Quaternion newPlayerRotation = Quaternion.Euler(
                0,
                moduleCamera.transform.rotation.eulerAngles.y,
                moduleCamera.transform.rotation.eulerAngles.z
            );

            Quaternion newCameraRotation = Quaternion.Euler(
                moduleCamera.transform.rotation.eulerAngles.x,
                0,
                0
            );

            movePlayer.RaiseEvent(new PlayerTransformPayload
            {
                Position = finalPosition,
                Rotation = (newPlayerRotation),
                CameraRotation = (newCameraRotation)
            });
        }
        else
        {
            Debug.LogWarning("Screen module transition raycast failed");
        }
    }
}
