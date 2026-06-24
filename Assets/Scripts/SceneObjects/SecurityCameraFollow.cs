using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecurityCameraFollow : MonoBehaviour, IInteractable
{
    //Required for Interaction
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
    private Outline outline;
    [SerializeField] private string requiredItem = "CoffeeCupFull";
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private VoidEventChannelSO clearCrosshair;
    [SerializeField] private VoidEventChannelSO thumbsUpCrosshair;
    [SerializeField] private VoidEventChannelSO cameraDisabled;
    


    [SerializeField] private float rotationSpeed = 2f;
    private readonly Quaternion rotationOffset = Quaternion.Euler(-90f, 180f, 0f);
    private Transform mainCameraTransform;
    private Transform disablePointTransform;
    private int state = 0; //0 is active, 1 is disabled, 2 is a unique "force splash" state for day 2.
    [SerializeField] private int dayForceSplash;
    [SerializeField] private int[] reActivateTimer;
    Transform coffee;
    Transform sparks;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = 5;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }

        //Set the two transforms for the camera to switch between
        disablePointTransform = transform.parent.Find("DisablePoint");
        mainCameraTransform = Camera.main.transform;

        //Grab references to our two children so we can toggle them (Cofee Drip and Sparks)
        coffee = transform.Find("Cofee Drip");
        sparks = transform.Find("Sparks");
        //Make sure we can grab the level manager state
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            if(LevelManager.Instance.currentSession.currentDay==dayForceSplash) //If we are on designated day
            {
                state = 2;
                sparks.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        Vector3 directionToLook;
        if(state!=1)
        {
            directionToLook = mainCameraTransform.position - transform.position;
        }
        else
        {
            directionToLook = disablePointTransform.position - transform.position;
        }

        if(directionToLook!=null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up) * rotationOffset;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    public int getState()
    {
        return state;
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
        //Debug.Log($"Needs: {requiredItem}");
        //Debug.Log($"Currently Holding: {currentItemHeld.name}");
        if (currentItemHeld != null && (state==0||state==2))
        {
            return requiredItem.Equals(currentItemHeld.name);
        }
        //THIS SHOULD BE FALSE
        return false;
    }

        public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        if (currentItemHeld != null)
        {
            lastItemheld = currentItemHeld;
        }
    }

    public void StartInteract()
    {
        //Debug.Log("AAAAAAAA");
        if(state!=1)
        {
            state = 1;
            coffee.gameObject.SetActive(true);
            cameraDisabled.RaiseEvent();
            Invoke("reEnableByDayTimer", reActivateTimer[LevelManager.Instance.currentSession.currentDay-1]);
        }
    }

    private void reEnableByDayTimer()
    {
        if(LevelManager.Instance.currentSession.currentDay==dayForceSplash) //If we are on designated day
        {
            state = 2;
            coffee.gameObject.SetActive(false);
        }
        else{
            state = 0;
            coffee.gameObject.SetActive(false);
        }
    }




    public void StopInteract()
    {
    }

    public void StartHover()
    {
        if (thumbsUpCrosshair != null)
        {
            thumbsUpCrosshair.RaiseEvent();
        }
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
        //Debug.Log("Hovering On Camera!");
    }

    public void StopHover()
    {
        if (clearCrosshair != null)
        {
            clearCrosshair.RaiseEvent();
        }
        outline.enabled = false;
        //Debug.Log("No Longer Hovering!");
    }

}