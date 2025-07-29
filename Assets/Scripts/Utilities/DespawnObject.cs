using UnityEngine;
using System.Collections;

public class DespawnObject : MonoBehaviour
{
    public void StartDespawn(float despawnTime, float despawnDelay)
    {
        StartCoroutine(Despawn(despawnTime, despawnDelay));
    }

    public void StartDespawn(float despawnTime)
    {
        StartCoroutine(Despawn(despawnTime, 0f));
    }

    private IEnumerator Despawn(float despawnTime, float despawnDelay)
    {
        yield return new WaitForSeconds(despawnDelay);
        
        int childCount = transform.childCount;
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            
            yield return new WaitForSeconds(despawnTime / childCount);
        }

        Destroy(this);
    }
}