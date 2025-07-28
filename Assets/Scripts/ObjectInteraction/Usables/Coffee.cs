using UnityEngine;

public class Coffee : MonoBehaviour, IUsable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private GameObject emptyCupPrefab;

    public void UseItem()
    {
        stopInteraction.RaiseEvent();

        GameObject emptyCup = Instantiate(emptyCupPrefab, transform.position, transform.rotation);
        emptyCup.transform.localScale = transform.localScale;
        emptyCup.name = emptyCupPrefab.name;

        Destroy(gameObject);
    }
}