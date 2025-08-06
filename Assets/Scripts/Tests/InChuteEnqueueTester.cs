using UnityEngine;

public class InChuteEnqueueTester : MonoBehaviour
{
    [SerializeField] private BoxItemsSO testBoxItems;
    private InChute chute;

    void Awake()
    {
        chute = GetComponent<InChute>();
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            chute.EnqueueItems(testBoxItems);
        }
    }
}
