using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAchievement
{
    BeatDay1, //Implemented
    BeatDay2, //Implemented
    BeatDay3, //Implemented
    BeatDay4, //Implemented
    BeatDay5, //Implemented
    BeatDay6, //Implemented
    BeatDay7, //Implemented
    BeatDay8, //Implemented
    BeatDay9, //Implemented
    Planet1GoodEnding,
    Planet2GoodEnding,
    Planet3GoodEnding,
    NeutralEnding, //Implemented
    HPCEnding, //Implemented
    CMSEnding, //Implemented
    AntiScienceEnding, //Implemented
    ScienceEnding, //Implemented
    TooSlowLoss, //Implemented
    PoorEfficiencyLoss, //Implemented
    CompromisedBuildLoss, //Implemented
    OneThousandCups,
    BuildUnder3Mins,
    AvgBuildUnder5mins,
    GameCompleteInUnder25Mins,
    Drink10Coffee,
    CoffeeWhileMaxCorrosion,
    GetEfficiencyPity,
    GetMaxEfficiencyPity,
    BeatDayUnder100,
    FirstCMSBuild,
    SplashCamera,
    AllAchievements,
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    private uint appID = 4883240;
    private int totalAchievementNum = 32;
    private bool connectedToSteam = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        try
        {
            Steamworks.SteamClient.Init(appID);
            connectedToSteam = true;
        }
        catch(System.Exception exception)
        {
            Debug.LogWarning($"Failed to connect to steam! Details: {exception}");
            connectedToSteam = false;
        }
    }

    void Update()
    {
        if(connectedToSteam)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }

    public void DisconnectFromSteam()
    {
        if(connectedToSteam)
        {
            Steamworks.SteamClient.Shutdown();
        }
    }

    public void unlockAchievement(eAchievement _AchievementToUnlock)
    {
        if(connectedToSteam)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + (int)_AchievementToUnlock);
            ach.Trigger();
            CheckForPlatinumAchievement();
        }
    }

    //An overload that allows the calling of achievements by number rather than enum, used for things like the Day achievements
    public void unlockAchievement(int _AchievementToUnlock)
    {
        if(connectedToSteam)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + _AchievementToUnlock);
            ach.Trigger();
            CheckForPlatinumAchievement();
        }
    }

    private void CheckForPlatinumAchievement()
    {
        int numberOfAchievementsRequired = totalAchievementNum - 1;
        int numberOfUnlockedAchievements = 0; //Calculated in loop

        for(int i=0;i<numberOfAchievementsRequired;i++)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + i);
            if(ach.State == true)
            {
                numberOfUnlockedAchievements++;
            }
        }

        if(numberOfUnlockedAchievements == numberOfAchievementsRequired)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + (int)eAchievement.AllAchievements);
            ach.Trigger();
        }
    }


}
