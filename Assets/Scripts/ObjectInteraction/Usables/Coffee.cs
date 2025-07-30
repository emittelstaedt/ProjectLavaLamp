using UnityEngine;

public class Coffee : MonoBehaviour, IUsable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private IntEventChannelSO changePostProcess;
    [SerializeField] private GameObject emptyCupPrefab;

    public void UseItem()
    {
        stopInteraction.RaiseEvent();
        changePostProcess.RaiseEvent((int) PostProcess.Clean);

        GameObject emptyCup = Instantiate(emptyCupPrefab, transform.position, transform.rotation);
        emptyCup.transform.localScale = transform.localScale;
        emptyCup.name = emptyCupPrefab.name;

        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.CoffeeSlurp, .5f);

        Destroy(gameObject);
    }
}