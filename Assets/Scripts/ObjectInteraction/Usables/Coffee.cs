using UnityEngine;
using UnityEngine.SceneManagement;

public class Coffee : MonoBehaviour, IUsable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private IntEventChannelSO changePostProcess;
    [SerializeField] private VoidEventChannelSO coffeeDrank;
    [SerializeField] private GameObject emptyCupPrefab;
    private bool wasHeldRecently = false;

    private void Awake()
    {
        //Debug.Log("AAAAAAAAAAAAAAAA IM REAL I SWEAR");
        Invoke(nameof(checkHeldStatus), 0.3f); 
    }

    public void UseItem()
    {

        // Pull the data out of the Level Manager
        if (LevelManager.Instance != null && LevelManager.Instance.currentSession != null)
        {
            LevelManager.Instance.currentSession.coffeeDrank++;
            //Debug.Log($"Current Coffees Drank is: {LevelManager.Instance.currentSession.coffeeDrank}");
        }
        else
        {
            Debug.LogWarning("Coffee could not find levelmanager!");
        }

        stopInteraction.RaiseEvent();
        changePostProcess.RaiseEvent((int) PostProcess.Clean);
        coffeeDrank.RaiseEvent();

        GameObject emptyCup = Instantiate(emptyCupPrefab, transform.position, transform.rotation);
        emptyCup.transform.localScale = transform.localScale;
        emptyCup.name = emptyCupPrefab.name;
		SceneManager.MoveGameObjectToScene(emptyCup, SceneManager.GetSceneByName("OfficeWorkplace"));
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.CoffeeSlurp, .5f);

        Destroy(gameObject);
    }

    public void emptyThisCup(){
        if(wasHeldRecently)
        {
        GameObject emptyCup = Instantiate(emptyCupPrefab, transform.position, transform.rotation);
        emptyCup.transform.localScale = transform.localScale;
        emptyCup.name = emptyCupPrefab.name;
		SceneManager.MoveGameObjectToScene(emptyCup, SceneManager.GetSceneByName("OfficeWorkplace"));
        Destroy(gameObject);
        }
    }

    private void checkHeldStatus()
    {
        //Debug.Log($"Was held recently?: {wasHeldRecently}");
        if(gameObject.CompareTag("Held")&&!wasHeldRecently)
        {
            wasHeldRecently = true;
            Invoke(nameof(checkHeldStatus), 0.5f); 
        }
        else if(!gameObject.CompareTag("Held")&&wasHeldRecently)
        {
            Invoke(nameof(heldDecay), 0.5f); 
        }
        Invoke(nameof(checkHeldStatus), 0.3f);
    }

    private void heldDecay()
    {
        wasHeldRecently = false;
        Invoke(nameof(checkHeldStatus), 0.3f); 
    }

}