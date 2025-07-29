using UnityEngine;
using System.Collections;

public class DespawnObject : MonoBehaviour
{
    public void StartDespawn(float despawnDelay, float despawnTime)
    {
        StartCoroutine(Despawn(despawnDelay, despawnTime));
    }

    private IEnumerator Despawn(float despawnDelay, float despawnTime)
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