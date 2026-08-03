using UnityEngine;
using System.Collections;

public class MinigameManager : MonoBehaviour
{
    [Tooltip("Used for quick balancing.")]
    [SerializeField] int effLossMegaFail;

    [Tooltip("The delay between sirens (excluding modifiers) depending on the day it is. 0 for none.")]
    [SerializeField] int[] DayExpectedTriggerWait;

    [SerializeField] int dayEnableMinigame1;
    [SerializeField] int dayEnableMinigame2;
    [SerializeField] int dayEnableMinigame3;
    [SerializeField] int dayEnableMinigame4;

    [Tooltip("An event channel for each siren.")]
	[SerializeField] private VoidEventChannelSO[] sirenChannels;

	[Tooltip("Signal for modifying efficiency speed:")]
	[SerializeField] private IntEventChannelSO modifySpeed;
	
	[Tooltip("Signal for directly modifying efficiency score:")]
	[SerializeField] private IntEventChannelSO modifyValue;

    public static MinigameManager Instance = null;
    EmployeeData currentSession = null;

    // terminalSirensAreActive[0] is terminal 2, terminalSirensAreActive[1] is terminal 3...
    bool[] terminalSirensAreActive = {false,false,false,false};

    bool hasGuaranteedNewMinigame = false;
    int guaranteedNewMinigame = -1;
	private bool dayBegun;
	
    [SerializeField] GameObject[] blocks;

    [SerializeField] private float volume = 1f;

    // Awake is called when object is made active
    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<MinigameManager>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
		dayBegun = false;
        //Make absolutely sure we aren't creating duplicate coroutines
        StopAllCoroutines();

        // Pull the data out of the Level Manager
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            currentSession = LevelManager.Instance.currentSession;
            //Debug.Log($"Minigame pulled data for: {currentSession.employeeName} on day {currentSession.currentDay}.");
            //Check to make sure we aren't on day 1, and that we have a corresponding serializefield for the day
            if(currentSession.currentDay != 0 && DayExpectedTriggerWait.Length >= currentSession.currentDay)
            {
                if(currentSession.currentDay < dayEnableMinigame1)
                {
                    blocks[0].SetActive(true);
                }
                if(currentSession.currentDay < dayEnableMinigame2)
                {
                    blocks[1].SetActive(true);
                }
                if(currentSession.currentDay < dayEnableMinigame3)
                {
                    blocks[2].SetActive(true);   
                }
                if(currentSession.currentDay < dayEnableMinigame4)
                {
					blocks[3].SetActive(true);
                }

                //If there's a new minigame today, setup so it will always trigger first.
                if(currentSession.currentDay == dayEnableMinigame1)
                {
                    guaranteedNewMinigame = 0;
                    hasGuaranteedNewMinigame = true;
                }
                else if(currentSession.currentDay == dayEnableMinigame2)
                {
                    guaranteedNewMinigame = 1;
                    hasGuaranteedNewMinigame = true;
                }
                else if(currentSession.currentDay == dayEnableMinigame3)
                {
                    guaranteedNewMinigame = 2;
                    hasGuaranteedNewMinigame = true;
                }
                else if(currentSession.currentDay == dayEnableMinigame4)
                {
                    guaranteedNewMinigame = 3;
                    hasGuaranteedNewMinigame = true;
                }
                else
                {
                    guaranteedNewMinigame =- 1;
                    hasGuaranteedNewMinigame = false;
                }
            }       
        }
        else
        {
            Debug.LogError("Couldn't pull session data from levelmanager! Minigames will not be triggered.");
        }
    }

	public void beginDay()
	{
		if(dayBegun == false)
		{
			Invoke("nextTriggerTimer", DayExpectedTriggerWait[currentSession.currentDay-1]);
			dayBegun = true;
		}
	}
	
    void nextTriggerTimer()
    {
        if(sirenChannels != null){ //Nullcheck
            //Randomly pick the next game we're going to alert
            //THIS WILL NEED TO ACCOUNT FOR THE DAY WE'RE ON TO EXCLUDE UNLOCKING GAMES EARLY
            int nextGame = 0;

            //If there's a new minigame today, trigger it first.
            if(hasGuaranteedNewMinigame)
            {
                nextGame = guaranteedNewMinigame;
                switch(nextGame){
                    case 0:
                    AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, SoundType.NewMiniGame3, volume);
                    break;
                    case 1:
                    AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, SoundType.NewMiniGame2, volume);
                    break;
                    case 2:
                    AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, SoundType.NewMiniGame1, volume);
                    break;
                    case 3:
                    AudioManager.Instance.PlayQueuedSound(AudioQueue.Announcement, MixerType.SFX, SoundType.NewMiniGame4, volume);
                    break;
                }
                hasGuaranteedNewMinigame = false;
            }
            else//Default case of randomization
            {
                nextGame = findNextTriggerableMinigame();
            }



            //Trigger corresponding event with sirenChannels[nextGame]
            if(sirenChannels.Length<=nextGame){
                Debug.LogWarning("No corresponding siren channel found! List too short!");
                return;
            }
            else if(nextGame==-1){
                //Debug.Log("No triggerable minigame found!");
            }
            else{
                sirenChannels[nextGame].RaiseEvent();
            }

            //Randomize with currentSession.efficiency and rand

            // Normalize efficiency score between max and min multiplier
            float effMultiplier = currentSession.efficiency * 0.0003f + 0.7f;
            //Debug.Log($"Current mult with efficiency is: {effMultiplier}");

            // Random modifier (-10 to 10 seconds added randomly)
            int rawRandom = UnityEngine.Random.Range(-10, 11);

            //Debug.Log($"Expected minigame timer is: {DayExpectedTriggerWait[currentSession.currentDay-1]*effMultiplier+rawRandom}");
            //Recursive invoke, multiply by efficiency, add random seconds
            Invoke("nextTriggerTimer", DayExpectedTriggerWait[currentSession.currentDay-1]*effMultiplier+rawRandom);
        }
        else{
            Debug.LogWarning("No siren channels found!");
        }
    }

    int findNextTriggerableMinigame(){

        if(!isAnyAvailableSirens()){
            modifyValue.RaiseEvent(effLossMegaFail);
            //Debug.Log("PAIN AND SUFFERING UPON YE");
            return -1;
        }

        int range = 0;
        int currentChoice = -1;
        //Restrict minigame choice by current day
        if(currentSession.currentDay>=dayEnableMinigame1){
            range++;
        }
        if(currentSession.currentDay>=dayEnableMinigame2){
            range++;
        }
        if(currentSession.currentDay>=dayEnableMinigame3){
            range++;
        }
        if(currentSession.currentDay>=dayEnableMinigame4){
            range++;
        }
        //Reroll choice until landing on an inactive terminal
        while(currentChoice<0||terminalSirensAreActive[currentChoice]){
            currentChoice = UnityEngine.Random.Range(0, range); 
        }
        //Return our choice without duplicates and adhering to dayEnableMinigame scheme
        return currentChoice;
    }

    bool isAnyAvailableSirens(){
        bool isThere = false;
        //Check available sirens against current day to determine if a minigame is available in relation to the day
        if(currentSession.currentDay<=0){
            return false;
        }
        else if(currentSession.currentDay>=dayEnableMinigame1&&!terminalSirensAreActive[0]){
            isThere=true;
        }
        else if(currentSession.currentDay>=dayEnableMinigame2&&(!terminalSirensAreActive[0]||!terminalSirensAreActive[1])){
            isThere=true;
        }
        else if(currentSession.currentDay>=dayEnableMinigame3&&(!terminalSirensAreActive[0]||!terminalSirensAreActive[1]||!terminalSirensAreActive[2])){
            isThere=true;
        }
        else if(currentSession.currentDay>=dayEnableMinigame4&&(!terminalSirensAreActive[0]||!terminalSirensAreActive[1]||!terminalSirensAreActive[2]||!terminalSirensAreActive[3])){
            isThere=true;
        }


        return isThere;
    }

    //How we track when our own signals are sent out to start a minigame siren
    public void siren2TurnedOn(){
        terminalSirensAreActive[0] = true;
		modifySpeed.RaiseEvent(1);
    }
    public void siren3TurnedOn(){
        terminalSirensAreActive[1] = true;
		modifySpeed.RaiseEvent(1);
    }
    public void siren4TurnedOn(){
        terminalSirensAreActive[2] = true;
		modifySpeed.RaiseEvent(1);
    }
    public void siren5TurnedOn(){
        terminalSirensAreActive[3] = true;
		modifySpeed.RaiseEvent(1);
    }

    //How we track when minigames have been completed
    public void siren2TurnedOff(){
        if(terminalSirensAreActive[0])
        {
            modifyValue.RaiseEvent(25);
            terminalSirensAreActive[0] = false;
			modifySpeed.RaiseEvent(-1);
        }
    }
    public void siren3TurnedOff(){
        if(terminalSirensAreActive[1])
        {
            modifyValue.RaiseEvent(25);
            terminalSirensAreActive[1] = false;
			modifySpeed.RaiseEvent(-1);
        }
    }
    public void siren4TurnedOff(){
        if(terminalSirensAreActive[2]){
            modifyValue.RaiseEvent(25);
            terminalSirensAreActive[2] = false;
			modifySpeed.RaiseEvent(-1);
        }
    }
    public void siren5TurnedOff(){
        if(terminalSirensAreActive[3]){
            modifyValue.RaiseEvent(25);
            terminalSirensAreActive[3] = false;
			modifySpeed.RaiseEvent(-1);
        }
    }

}
