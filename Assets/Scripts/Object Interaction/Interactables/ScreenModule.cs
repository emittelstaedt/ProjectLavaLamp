using UnityEngine;
using UnityEngine.InputSystem; 

public class ScreenModule : MonoBehaviour, IInteractable
{
    [SerializeField] private Camera moduleCamera;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private float distanceFromCamera = 0.5f;
    private Outline outline;
    private Camera mainCamera;
    private Transform playerTransform;
    private MeshRenderer playerMesh;
    
    public void Awake()
    {
        mainCamera = Camera.main;
        playerTransform = mainCamera.transform.parent;
        playerMesh = playerTransform.gameObject.GetComponent<MeshRenderer>();
    
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
    
    public void StartInteract()
    {
        InputSystem.actions.FindActionMap("Player").Disable();
        playerMesh.enabled = false;
        
        CameraSwapper.Instance.SwapCameras(mainCamera, moduleCamera, EnablePlayerInteract);
        PutPlayerInFrontOfScreen();
    }
    
    public void StopInteract()
    {
        InputSystem.actions.FindAction("Interact").Disable();
        playerMesh.enabled = true;
        
        CameraSwapper.Instance.SwapCameras(moduleCamera, mainCamera, EnablePlayerControls);
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
    
    private void EnablePlayerInteract()
    {
        InputSystem.actions.FindAction("Interact").Enable();
    }
    
    private void EnablePlayerControls()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
    }
    
    private void PutPlayerInFrontOfScreen()
    {
        playerTransform.position = moduleCamera.transform.position - moduleCamera.transform.rotation * Vector3.forward * distanceFromCamera;
        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, -Vector3.up, out hit))
        {
            playerTransform.position -= Vector3.up * hit.distance;
        }
        else
        {
            Debug.LogWarning("Screen module transition raycast failed");
        }

        Vector3 newPlayerRotation = moduleCamera.transform.rotation.eulerAngles;
        Vector3 newPlayerCameraRotation = new Vector3(newPlayerRotation.x, 0, 0);
        newPlayerRotation.x = 0;

        playerTransform.rotation = Quaternion.Euler(newPlayerRotation);
        mainCamera.transform.localRotation = Quaternion.Euler(newPlayerCameraRotation);
    }
}
