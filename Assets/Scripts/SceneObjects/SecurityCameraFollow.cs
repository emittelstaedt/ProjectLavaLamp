using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecurityCameraFollow : MonoBehaviour, IInteractable
{
    //Required for Interaction
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
    //private Outline outline;
    [SerializeField] private string requiredItem = "CoffeeCupFull";
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private VoidEventChannelSO defaultCrosshair;
    [SerializeField] private VoidEventChannelSO thumbsUpCrosshair;
    [SerializeField] private VoidEventChannelSO cameraDisabled;
    [SerializeField] private VoidEventChannelSO clearCoffee;

    [SerializeField] private float rotationSpeed = 2f;
    private readonly Quaternion rotationOffset = Quaternion.Euler(-90f, 180f, 0f);
    private Transform mainCameraTransform;
    private Transform disablePointTransform;
    private Transform transitionPointTransform;

    // 0 = active/arrived (looks at player), 1 = disabled, 2 = force-splash (day 2), 3 = transitioning between rooms.
    // This is now a REPORTED label derived each frame from comparing pathT/lookBlend
    // against targetAtPointB - not something set manually to drive behavior.
    private int state = 0;

    [SerializeField] private int dayForceSplash;
    [SerializeField] private int[] reActivateTimer;
    Transform coffee; //Reference to particle child
    Transform sparks; //Reference to particle child
    private Material objectMaterial; //Grabs own shader
    private int coffeeShaderAmount; //Used to modify own shader's slider value

    [Header("Transition - Look")]
    [SerializeField] private float lookTransitionSpeed = 2f;
    [SerializeField] private float lookDownSpeedMultiplier = 3f; // how much faster the initial look-down is vs. the return look
    private float lookBlend; // 0 = looking at player/camera, 1 = looking at TransitionPoint

    [Header("Transition - Movement")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float archHeight = 2f;
    [SerializeField] private float moveSpeed = 1f; // units/sec along the arc

    // The single source of truth for "which room the rig should be in." Always current,
    // updated by signals even while disabled - only the physical execution toward it pauses.
    private bool targetAtPointB;
    private float pathT; // 0 = at pointA, 1 = at pointB
    private Vector3[] waypoints;
    private float[] cumulativeDist;
    private float totalPathLength;

    [Tooltip("Value that efficiency will be decreased by once signal is received.")]
	[SerializeField] private IntEventChannelSO modifySpeed;

    [SerializeField] private SoundType onCameraRestart;
    [SerializeField] private SoundType onCameraSplashed;
    [SerializeField] private SoundType onBoxBroken;

    private bool isAngery = false;
    private bool isHappe = false;

    [SerializeField] private GameObject lightOfDoomAndEfficiencyLoss;
    bool hasPlayedWarningSound = false;


    private void Awake()
    {
        StopAllCoroutines();

        //Set the transforms for the camera to switch its look target between
        disablePointTransform = transform.parent.Find("DisablePoint");
        transitionPointTransform = transform.parent.Find("TransitionPoint");
        mainCameraTransform = Camera.main.transform;

        //Grab references to our two children so we can toggle them (Cofee Drip and Sparks)
        coffee = transform.Find("Cofee Drip");
        sparks = transform.Find("Sparks");
        //Make sure we can grab the level manager state
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            if (LevelManager.Instance.currentSession.currentDay == dayForceSplash) //If we are on designated day
            {
                state = 2;
                sparks.gameObject.SetActive(true);
            }
        }

        objectMaterial = GetComponent<Renderer>().material;
        coffeeShaderAmount = Shader.PropertyToID("_Coffee_amount");
        objectMaterial.SetColor("_Emiss_ON_Color", Color.yellow);

        BuildTransitionPath();
    }

    private void BuildTransitionPath()
    {
        Vector3 upA = pointA.position + Vector3.up * archHeight;
        Vector3 upB = pointB.position + Vector3.up * archHeight;
        waypoints = new[] { pointA.position, upA, upB, pointB.position }; // up, over, down

        cumulativeDist = new float[waypoints.Length];
        for (int i = 1; i < waypoints.Length; i++)
            cumulativeDist[i] = cumulativeDist[i - 1] + Vector3.Distance(waypoints[i - 1], waypoints[i]);

        totalPathLength = cumulativeDist[^1];

        // Assume the rig starts parked at pointA until told otherwise
        pathT = 0f;
        transform.parent.position = waypoints[0];
    }

    private void Update()
    {
        if (state==1)
        {
            // are simply never touched this frame, and the rig looks at the disable point
            // as the visual "something's wrong" cue in both cases.
            Vector3 disabledLookDir = disablePointTransform.position - transform.position;
            Quaternion disabledRotation = Quaternion.LookRotation(disabledLookDir, Vector3.up) * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, disabledRotation, rotationSpeed * Time.deltaTime);
            return;
        }

        UpdateLook();
        UpdateMovement();
        UpdateStateLabel();
    }

    private void UpdateLook()
    {
        // Look down at the transition point whenever we haven't arrived at the target room yet,
        // look at the player once we have. This is derived fresh each frame, not commanded.
        bool arrivedAtTarget = Mathf.Approximately(pathT, targetAtPointB ? 1f : 0f);
        float targetBlend = arrivedAtTarget ? 0f : 1f;
        lookBlend = Mathf.MoveTowards(lookBlend, targetBlend, lookTransitionSpeed * Time.deltaTime);

        Vector3 toCamera = mainCameraTransform.position - transform.position;
        Vector3 toTransitionPoint = transitionPointTransform.position - transform.position;
        Vector3 directionToLook = Vector3.Slerp(toCamera.normalized, toTransitionPoint.normalized, lookBlend);
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up) * rotationOffset;

        // Turning to look at the transition point happens fast (mechanical, intended);
        // turning back to track the player afterward uses normal speed.
        bool lookingDown = targetBlend > lookBlend;
        float effectiveRotationSpeed = lookingDown ? rotationSpeed * lookDownSpeedMultiplier : rotationSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, effectiveRotationSpeed * Time.deltaTime);
    }

    private void UpdateMovement()
    {
        float targetT = targetAtPointB ? 1f : 0f;

        // Gate: don't start physically moving until the look-down blend has essentially finished
        bool lookFinished = lookBlend >= 0.99f;
        if (lookFinished)
        {
            float maxDelta = (moveSpeed * Time.deltaTime) / Mathf.Max(totalPathLength, 0.0001f);
            pathT = Mathf.MoveTowards(pathT, targetT, maxDelta);
        }

        transform.parent.position = EvaluatePathPosition(pathT);
    }

    private void UpdateStateLabel()
    {
        bool arrived = Mathf.Approximately(pathT, targetAtPointB ? 1f : 0f) && lookBlend <= 0.01f;

        bool isSplashDay = LevelManager.Instance != null && LevelManager.Instance.currentSession != null
            && LevelManager.Instance.currentSession.currentDay == dayForceSplash;

        //Check if we are coming out of a mobile/disabled state, if we are, play the reboot noise.
        //Debug.Log("Updating state label...");
        if(((arrived ? (isSplashDay ? 2 : 0) : 3) == 2 || (arrived ? (isSplashDay ? 2 : 0) : 3) == 0) && (state == 1 || state == 3))
        {
            //Debug.Log("Playing cameraRestart Noise!");
            //Only play restart noise if camera is angry due to cms box breaking. This also prevents overlay with announcement on day 1.
            if(isAngery)
            {
                if(!hasPlayedWarningSound)
                {
                    AudioManager.Instance.PlaySound(MixerType.SFX, onBoxBroken, 1f, transform.position); 
                    hasPlayedWarningSound = true;
                }
                else
                {
                    AudioManager.Instance.PlaySound(MixerType.SFX, onCameraRestart, 1f, transform.position);
                }
                objectMaterial.SetColor("_Emiss_ON_Color", Color.red);
                if(lightOfDoomAndEfficiencyLoss!=null)
                {
                    lightOfDoomAndEfficiencyLoss.SetActive(true);
                }
                
            }
        }

        // Idle label depends on which "idle" applies today - splash day idles at 2, otherwise 0.
        // Actively correcting toward the target always reports 3, regardless of which day it is.
        state = arrived ? (isSplashDay ? 2 : 0) : 3;
    }

    private Vector3 EvaluatePathPosition(float t)
    {
        float targetDist = t * totalPathLength;
        for (int i = 1; i < cumulativeDist.Length; i++)
        {
            if (targetDist <= cumulativeDist[i] || i == cumulativeDist.Length - 1)
            {
                float segStart = cumulativeDist[i - 1];
                float segLen = cumulativeDist[i] - segStart;
                float segT = segLen > 0f ? (targetDist - segStart) / segLen : 0f;
                return Vector3.Lerp(waypoints[i - 1], waypoints[i], segT);
            }
        }
        return waypoints[^1];
    }

    /// <summary>
    /// Call this from whatever event channel raises the "switch rooms" signal.
    /// Deliberately NOT gated on state - the rig should always know the correct
    /// answer to "which room should I be in," even while disabled. Only the
    /// physical execution toward that target pauses during disable; the target
    /// itself is always live. If two signals arrive back-to-back while disabled
    /// (e.g. move to B, then back to A), only the net result matters - whichever
    /// call came last is what the rig corrects toward once re-enabled.
    /// </summary>
    public void ReceiveTransitionSignal(bool moveToPointB)
    {
        targetAtPointB = moveToPointB;
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
        if (currentItemHeld != null && (state == 0 || state == 2 || state == 3))
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
        if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.SplashCamera);}
		if(isAngery)
		{
			modifySpeed.RaiseEvent(-1);
		}
        clearCoffee.RaiseEvent();
        //Debug.Log("AAAAAAAA");
        if (state != 1)
        {
            state = 1; //Set to disabled
			sparks.gameObject.SetActive(false);
            coffee.gameObject.SetActive(true); //Set our coffee drip particles active
            cameraDisabled.RaiseEvent(); //Send a signal (mainly for door to receive to know to open)
            AudioManager.Instance.PlaySound(MixerType.SFX, onCameraSplashed, 1f, transform.position);
            //Set light to green, get rid of spotlight
            objectMaterial.SetColor("_Emiss_ON_Color", Color.green);
            if(lightOfDoomAndEfficiencyLoss!=null)
            {
                lightOfDoomAndEfficiencyLoss.SetActive(false);
            }

            objectMaterial.SetFloat(coffeeShaderAmount, 1);
            //Make sure we renable according to the right timer (30s if cannot find day)
            if(LevelManager.Instance!=null)
            {
                StartCoroutine(DecayRoutine());
                Invoke("reEnableByDayTimer", reActivateTimer[LevelManager.Instance.currentSession.currentDay - 1]);
            }
            else
            {
                Invoke("reEnableByDayTimer", 60);
            }
        }
    }

    private void reEnableByDayTimer()
    {
        coffee.gameObject.SetActive(false);
        if(isAngery)
        {
			modifySpeed.RaiseEvent(1);
            objectMaterial.SetColor("_Emiss_ON_Color", Color.red);
            if(lightOfDoomAndEfficiencyLoss!=null)
            {
                lightOfDoomAndEfficiencyLoss.SetActive(true);
            }
        }
        else if(isHappe)
        {
            objectMaterial.SetColor("_Emiss_ON_Color", Color.green);
            if(lightOfDoomAndEfficiencyLoss!=null)
            {
                lightOfDoomAndEfficiencyLoss.SetActive(false);
            }
        }
        else
        {
            objectMaterial.SetColor("_Emiss_ON_Color", Color.yellow);
            lightOfDoomAndEfficiencyLoss.SetActive(false);
        }
        if(LevelManager.Instance!=null)
        {
            state = (LevelManager.Instance.currentSession.currentDay == dayForceSplash) ? 2 : 0;   
        }
        else
        {
            state = 0;
        }
    }

    public void StopInteract() { }//Required for IInteractable

    public void StartHover()
    {
        if (thumbsUpCrosshair != null)
        {
            thumbsUpCrosshair.RaiseEvent();
        }
        //Debug.Log("Hovering On Camera!");
    }

    public void StopHover()
    {
        if (defaultCrosshair != null)
        {
            defaultCrosshair.RaiseEvent();
        }
        //Debug.Log("No Longer Hovering!");
    }

    private System.Collections.IEnumerator DecayRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < reActivateTimer[LevelManager.Instance.currentSession.currentDay - 1])
        {
            elapsedTime += Time.deltaTime;

            // Calculate the percentage of completion (0.0 to 1.0)
            float t = elapsedTime / reActivateTimer[LevelManager.Instance.currentSession.currentDay - 1];

            // Linearly interpolate from starting value down to 0
            float currentValue = Mathf.Lerp(1f, 0f, t);

            // Send the updated value to the shader
            objectMaterial.SetFloat("_Coffee_amount", currentValue);

            // Wait until the next frame
            yield return null;
        }

        //This checks to see if we're current in the room we want to be, only plays the sound if we are.
        if(Mathf.Approximately(pathT, targetAtPointB ? 1f : 0f) && lookBlend <= 0.01f)
        {
            if(!hasPlayedWarningSound&&isAngery)
            {
                AudioManager.Instance.PlaySound(MixerType.SFX, onBoxBroken, 1f, transform.position);
                hasPlayedWarningSound = true;
            }
            else
            {
                AudioManager.Instance.PlaySound(MixerType.SFX, onCameraRestart, 1f, transform.position);
            }
        }

        // Ensure it hits exactly 0 at the very end
        objectMaterial.SetFloat("_Coffee_amount", 0f);
    }


    public void CMSBoxBroken()
    {
        isAngery = true; //Always set this
		if(state != 1)
		{
			modifySpeed.RaiseEvent(1);
            objectMaterial.SetColor("_Emiss_ON_Color", Color.red);
            if(lightOfDoomAndEfficiencyLoss!=null)
            {
                lightOfDoomAndEfficiencyLoss.SetActive(true);
            }
            if(!hasPlayedWarningSound)
            {
                AudioManager.Instance.PlaySound(MixerType.SFX, onBoxBroken, 1f, transform.position);
                hasPlayedWarningSound = true;
            }
		}
    }

    public void coffeeDrank()
    {
        if(isAngery)
        {
            return; //We don't want this overriding the cms box break since efficiency will still tick down
        }
        if(LevelManager.Instance.currentSession.currentDay!=dayForceSplash){
            isHappe = true;
            objectMaterial.SetColor("_Emiss_ON_Color", Color.green);
        }

        if(lightOfDoomAndEfficiencyLoss!=null)
        {
            lightOfDoomAndEfficiencyLoss.SetActive(false);
        }
    }

}