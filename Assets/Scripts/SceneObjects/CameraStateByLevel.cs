using UnityEngine;

public class CameraStateByLevel : MonoBehaviour
{
    //List of days (ints) where the camera will be disabled (probably only day 2)
    [SerializeField] int[] daysDisabled;


    void Awake()
    {
                    // Pull the data out of the Level Manager
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            for(int i=0;i<daysDisabled.Length;i++){
                if(daysDisabled[i]==LevelManager.Instance.currentSession.currentDay){
                    gameObject.SetActive(false);
                    //Debug.Log("Camera disabled!");
                }
            }    
        }
        else{
            Debug.LogWarning("Camera could not get day from levelmanager!");
        }
    }
}
