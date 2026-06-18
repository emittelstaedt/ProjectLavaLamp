using UnityEngine;
using UnityEngine.SceneManagement;

public class Coffee : MonoBehaviour, IUsable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private IntEventChannelSO changePostProcess;
    [SerializeField] private VoidEventChannelSO coffeeDrank;
    [SerializeField] private GameObject emptyCupPrefab;

    public void UseItem()
    {
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
}