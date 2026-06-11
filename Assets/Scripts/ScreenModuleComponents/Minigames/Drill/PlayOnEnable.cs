using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Animation>().Play();
    }
}
