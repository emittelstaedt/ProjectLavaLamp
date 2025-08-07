using UnityEngine;

public class InChuteEnqueueTester : MonoBehaviour
{
    [SerializeField] private BoxItemsSO testBoxItems;
    [SerializeField] private BoxItemsSOEventChannelSO enqueueBoxItems;
    
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            enqueueBoxItems.RaiseEvent(testBoxItems);
        }
    }
}
