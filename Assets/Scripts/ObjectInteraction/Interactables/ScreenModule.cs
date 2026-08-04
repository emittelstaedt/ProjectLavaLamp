using UnityEngine;
using UnityEngine.InputSystem; 

public class ScreenModule : MonoBehaviour, IUsableInteractable
{
    [SerializeField] private VoidEventChannelSO startInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
    [SerializeField] private BoolEventChannelSO setCursorVisibility;
    [SerializeField] private GameObjectEventChannelSO changeObject;
    [SerializeField] private Camera moduleCamera;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private float distanceFromCamera = 0.5f;
	[SerializeField] private bool longRangeInteract;
	
    private Outline outline;
    private Camera mainCamera;
    private PlayerController playerController;
    private MeshRenderer playerMesh;
    private GameObject currentItemHeld;
    private bool isBeingUsed;
    private InputAction interactAction;
    private InputAction useItemAction;
    private InputActionMap playerActionMap;

    public void Start()
    {
        mainCamera = Camera.main;
        playerController = mainCamera.transform.GetComponentInParent<PlayerController>();
        playerMesh = playerController.gameObject.GetComponent<MeshRenderer>();

        interactAction = InputSystem.actions.FindAction("Interact");
        useItemAction = InputSystem.actions.FindAction("UseItem");
        playerActionMap = InputSystem.actions.FindActionMap("Player");

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
		if(longRangeInteract == true)
		{
			return Settings.InteractionDistance + 5;
		}
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

        foreach (var action in playerActionMap.actions)
        {
            if (action != interactAction && action != useItemAction)
                action.Disable();
        }

        playerMesh.enabled = false;
        changeObject.RaiseEvent(gameObject);
        CameraSwapper.Instance.SwapCameras(mainCamera, moduleCamera, EnablePlayerInteract);
        PutPlayerInFrontOfScreen();
    }
    
    public void StopInteract()
    {
        if(currentItemHeld!=null&&currentItemHeld==gameObject)
        {
            stopInteract.RaiseEvent();
            changeObject.RaiseEvent(null);
            setCursorVisibility.RaiseEvent(false);
            playerMesh.enabled = true;
            
            CameraSwapper.Instance.SwapCameras(moduleCamera, mainCamera, EnablePlayerControls);
            isBeingUsed = false;
        }
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
        //Debug.Log($"Holding {currentItemHeld}");
    }

    private void EnablePlayerInteract()
    {
        startInteract.RaiseEvent();
        setCursorVisibility.RaiseEvent(true);
    }
    
    private void EnablePlayerControls()
    {
		if(GameObject.FindGameObjectWithTag("Lose") != true)
		{
			playerActionMap.Enable();
		}
    }

    private void PutPlayerInFrontOfScreen()
    {
        Vector3 targetPosition = moduleCamera.transform.position - moduleCamera.transform.forward * distanceFromCamera;

        if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit))
        {
            playerController.SetFootPosition(targetPosition + Vector3.down * hit.distance);
        }
        else
        {
            Debug.LogWarning("Screen module transition raycast failed");
        }

        Vector3 newPlayerRotation = moduleCamera.transform.rotation.eulerAngles;
        Vector3 newPlayerCameraRotation = new (newPlayerRotation.x, 0, 0);
        newPlayerRotation.x = 0;

        playerController.transform.rotation = Quaternion.Euler(newPlayerRotation);
        mainCamera.transform.localRotation = Quaternion.Euler(newPlayerCameraRotation);
    }
}