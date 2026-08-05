using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAchievement
{
    BeatDay1,
    BeatDay2,
    BeatDay3,
    BeatDay4,
    BeatDay5,
    BeatDay6,
    BeatDay7,
    BeatDay8,
    BeatDay9,
    Planet1GoodEnding,
    Planet2GoodEnding,
    Planet3GoodEnding,
    NeutralEnding,
    HPCEnding,
    CMSEnding,
    AntiScienceEnding,
    ScienceEnding,
    TooSlowLoss,
    PoorEfficiencyLoss,
    CompromisedBuildLoss,
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

public class SteamManager : MonoBehaviour
{
    public static SteamManager instance;
    private uint appID = 4883240;
    private int totalAchievementNum = 32;
    private bool connectedToSteam = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
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
