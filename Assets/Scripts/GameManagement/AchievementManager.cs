using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    Planet1GoodEnding, //Implemented
    Planet2GoodEnding, //Implemented
    Planet3GoodEnding, //Implemented
    NeutralEnding, //Implemented
    HPCEnding, //Implemented
    CMSEnding, //Implemented
    AntiScienceEnding, //Implemented
    ScienceEnding, //Implemented
    TooSlowLoss, //Implemented
    PoorEfficiencyLoss, //Implemented
    CompromisedBuildLoss, //Implemented
    OneThousandCups, //Implemented
    BuildUnder3Mins, //Implemented
    AvgBuildUnder5mins, //Implemented
    GameCompleteInUnder25Mins, //Implemented
    Drink10Coffee, //Implemented
    CoffeeWhileMaxCorrosion, //Implemented
    GetEfficiencyPity, //Implemented
    GetMaxEfficiencyPity, //Implemented
    BeatDayUnder100, //Implemented
    FirstCMSBuild, //Implemented
    SplashCamera, //Implemented
    AllAchievements, //Implemented
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    private uint appID = 4883240;
    //private int totalAchievementNum = 32;
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

        if (Steamworks.SteamClient.IsValid)
        {
            connectedToSteam = true;
            return;
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

    private void OnApplicationQuit()
    {
        DisconnectFromSteam();
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
            var ach = new Steamworks.Data.Achievement("Ach_" + _AchievementToUnlock);
			ach.Trigger();
            CheckForPlatinumAchievement();
        }
		int achieveNum = (int)_AchievementToUnlock;
		if(!(achieveNum >= 12 && achieveNum <= 16))
		{
			if(achieveNum >= 17)
			{
				achieveNum = achieveNum - 5;
			}
			string currentPath = Application.persistentDataPath + "/employee" + LevelManager.Instance.currentSession.employeeNumber.ToString() + ".json"; 
			EmployeeData backUpProfile = new EmployeeData();
			if(File.Exists(currentPath))
			{
				string readJson = File.ReadAllText(currentPath);
				backUpProfile = JsonUtility.FromJson<EmployeeData>(readJson);
			}
			backUpProfile.achievements[achieveNum] = true;
			string writeJson = JsonUtility.ToJson(backUpProfile);
			File.WriteAllText(currentPath, writeJson);
			LevelManager.Instance.currentSession.achievements[achieveNum] = true;
		}
    }

    //An overload that allows the calling of achievements by number rather than enum, used for things like the Day achievements
    public void unlockAchievement(int _AchievementToUnlock)
    {
        if(connectedToSteam)
        {
            var ach = new Steamworks.Data.Achievement("Ach_" + _AchievementToUnlock);
            ach.Trigger();
            CheckForPlatinumAchievement();
        }
		int achieveNum = _AchievementToUnlock;
		if(!(achieveNum >= 12 && achieveNum <= 16))
		{
			if(achieveNum >= 17)
			{
				achieveNum = achieveNum - 5;
			}
			string currentPath = Application.persistentDataPath + "/employee" + LevelManager.Instance.currentSession.employeeNumber.ToString() + ".json"; 
			EmployeeData backUpProfile = new EmployeeData();
			if(File.Exists(currentPath))
			{
				string readJson = File.ReadAllText(currentPath);
				backUpProfile = JsonUtility.FromJson<EmployeeData>(readJson);
			}
			backUpProfile.achievements[achieveNum] = true;
			string writeJson = JsonUtility.ToJson(backUpProfile);
			File.WriteAllText(currentPath, writeJson);
			LevelManager.Instance.currentSession.achievements[achieveNum] = true;
		}
    }

	/*
    public void CheckForPlatinumAchievement()
    {
		if(connectedToSteam)
        {
			int numberOfAchievementsRequired = totalAchievementNum - 1;
			int numberOfUnlockedAchievements = 0; //Calculated in loop

			for(int i=0;i<numberOfAchievementsRequired;i++)
			{
				var ach = new Steamworks.Data.Achievement("Ach_" + i);
				if(ach.State == true)
				{
					numberOfUnlockedAchievements++;
				}
			}

			if(numberOfUnlockedAchievements == numberOfAchievementsRequired)
			{
				var ach = new Steamworks.Data.Achievement("Ach_" + (int)eAchievement.AllAchievements);
				ach.Trigger();
				string currentPath = Application.persistentDataPath + "/employee" + LevelManager.Instance.currentSession.employeeNumber.ToString() + ".json"; 
				EmployeeData backUpProfile = new EmployeeData();
				if(File.Exists(currentPath))
				{
					string readJson = File.ReadAllText(currentPath);
					backUpProfile = JsonUtility.FromJson<EmployeeData>(readJson);
				}
				backUpProfile.achievements[26] = true;
				string writeJson = JsonUtility.ToJson(backUpProfile);
				File.WriteAllText(currentPath, writeJson);
				LevelManager.Instance.currentSession.achievements[26] = true;
			}
		}
    }
	*/
	public  void CheckForPlatinumAchievement()
	{
		bool unlockedEndings = true;
		bool unlockedAchieves = true;
		string currentPath = Application.persistentDataPath + "/employee" + LevelManager.Instance.currentSession.employeeNumber.ToString() + ".json"; 
		EmployeeData currentProfile = new EmployeeData();
		if(File.Exists(currentPath))
		{
			string readJson = File.ReadAllText(currentPath);
			currentProfile = JsonUtility.FromJson<EmployeeData>(readJson);
		}
		for(int i = 0; i < 5; i++)
		{
			if(currentProfile.endings[i] == false)
			{
				unlockedEndings = false;
				break;
			}
		}
		for(int i = 0; i < 26; i++)
		{
			if(currentProfile.achievements[i] == false)
			{
				unlockedAchieves = false;
				break;
			}
		}
		if(unlockedAchieves == true && unlockedEndings == true)
		{
			currentProfile.achievements[26] = true;
			string writeJson = JsonUtility.ToJson(currentProfile);
			File.WriteAllText(currentPath, writeJson);
			LevelManager.Instance.currentSession.achievements[26] = true;
			if(connectedToSteam)
			{
				var ach = new Steamworks.Data.Achievement("Ach_31");
				ach.Trigger();
			}
		}
	}
}
