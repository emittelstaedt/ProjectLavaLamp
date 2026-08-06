using UnityEngine;
using System.Collections;

public class SetGlobalCorrosion : MonoBehaviour
{
    [SerializeField] private float corrosionTier1 = 0.3333f;
    [SerializeField] private float corrosionTier2 = 0.6666f;
    [SerializeField] private float corrosionTier3 = 1f;
    [SerializeField] private float startingCorrosion = 0f;
	[SerializeField] private float startingDisappear = 0f;
    [SerializeField] private float serializedDuration = 3f;
	[SerializeField] private VoidEventChannelSO triggerDisappear;
    //Sound Stuff
    private GameObject soundGameObject;
    bool coffeeHasBeenDrank = false;
    int numCoffeesDrankToday = 0;
    [SerializeField] float musicVolume = 0.01f;
    int startingCoffeeLevel = 3;

	
    private static readonly int CorrosionID = Shader.PropertyToID("_Damage_Amount");
	private static readonly int DisappearID = Shader.PropertyToID("_Fade_amount");
    
	private void Awake()
    {
        if(LevelManager.Instance.currentSession!=null)
        {
            switch (LevelManager.Instance.currentSession.coffeeLevel)
            {
                case 0:
                    startingCorrosion = corrosionTier3;
                    startingCoffeeLevel = 0;
                    break;
                case 1:
                    startingCorrosion = corrosionTier2;
                    startingCoffeeLevel = 1;
                    break;
                case 2:
                    startingCorrosion = corrosionTier1;
                    startingCoffeeLevel = 2;
                    break;
                default:
                    startingCorrosion = 0f;
                    startingCoffeeLevel = 3;
                    break;
            }
            Shader.SetGlobalFloat(CorrosionID, startingCorrosion);
			Shader.SetGlobalFloat(DisappearID, startingDisappear);

            Invoke(nameof(StartCorrespondingMusic), 0.7f);   
        }
        else
        {
            Debug.LogWarning("Corrosion setter could not find level manager!");
        }
    }

    public void onCoffeeDrank()
    {

        coffeeHasBeenDrank = true;
        numCoffeesDrankToday++;

        if(numCoffeesDrankToday>=10){ //Heart attack achievement
			if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.Drink10Coffee);}
		}

        //We use startingcoffeelevel instead because otherwise the levelmanager gets to modify the value first internally
        if(startingCoffeeLevel==0){
            if(AchievementManager.Instance!=null){AchievementManager.Instance.unlockAchievement(eAchievement.CoffeeWhileMaxCorrosion);}
            //Debug.LogWarning("Coffeeachievement");
        }
        //Debug.LogWarning($"{LevelManager.Instance.currentSession.coffeeLevel}");

        StartCoroutine(FadeOutCorrosion(serializedDuration)); // Fade over serializeduration seconds
    }

    public IEnumerator FadeOutCorrosion(float duration)
    {
        float start = startingCorrosion;
		float startDis = startingDisappear;
        float elapsed = 0f;

		triggerDisappear.RaiseEvent();
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            Shader.SetGlobalFloat(CorrosionID, Mathf.Lerp(start, 0f, t));
			Shader.SetGlobalFloat(DisappearID, Mathf.Lerp(startDis, 1f, t));
            yield return null;
        }

        Shader.SetGlobalFloat(CorrosionID, 0f);
        startingCorrosion = 0f;
		startingDisappear = 1f;
        StartCorrespondingMusic();
    }



    void StartCorrespondingMusic()
    {

        if(soundGameObject!=null)
        {
            soundGameObject.SetActive(false);
        }

        if(coffeeHasBeenDrank)
        {
           soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.CorrosionMusic0, musicVolume); 
        }
        else
        {
            //Debug.Log($"Coffee level is:{LevelManager.Instance.currentSession.coffeeLevel}");
            //Remember that coffeelevel is inverse of corrosion level :(
            switch (LevelManager.Instance.currentSession.coffeeLevel)
                {
                    case 0:
                        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.CorrosionMusic3, musicVolume*2);
                        //Debug.Log($"Switch read as 0.");
                        break;
                    case 1:
                        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.CorrosionMusic2, musicVolume);
                        //Debug.Log($"Switch read as 1.");
                        break;
                    case 2:
                        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.CorrosionMusic1, musicVolume);
                        //Debug.Log($"Switch read as 2.");
                        break;
                    default:
                        soundGameObject = AudioManager.Instance.PlaySoundLoop(MixerType.Music, SoundType.CorrosionMusic0, musicVolume);
                        //Debug.Log($"Switch read as 3.");
                        break;
                }
        }

        if(soundGameObject!=null)
        {
            soundGameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if(soundGameObject!=null)
        {
            soundGameObject.SetActive(false);
        } 
    }
}
