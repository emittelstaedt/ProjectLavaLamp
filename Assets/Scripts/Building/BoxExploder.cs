using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Fracture))]
public class BoxExploder : MonoBehaviour
{
    [SerializeField] private BoxItemsSO boxItems;
    [SerializeField] private float minimumSmashSpeed = 50f;
    [SerializeField] private float despawnDelay = 3f;
    [SerializeField] private float despawnTime = 0.5f;
    private Fracture fracture;
    private new Rigidbody rigidbody;
    private Vector3[] positions;
    private float[] deltaTimes;
    private int speedAccuracy = 7;
    private bool isHeld;

    void Awake()
    {
        fracture = GetComponent<Fracture>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void OnHeldItemChange(GameObject item)
    {
        if (item == gameObject)
        {
            isHeld = true;
            StartCoroutine(SavePositions());
        }
        else if (isHeld)
        {
            isHeld = false;

            if (GetCurrentSpeed() > minimumSmashSpeed)
            {
                InitiateSmash();
            }
        }
    }

    private void InitiateSmash()
    {
        GameObject[] items = boxItems.Items;
        for (int i = 0; i < items.Length; i++)
        {
            // Ensures objects fly in different directions.
            float maxDifference = 0.5f;
            Vector3 randomization = new(Random.Range(-maxDifference, maxDifference),
                                        Random.Range(-maxDifference, maxDifference),
                                        Random.Range(-maxDifference, maxDifference));

            Instantiate(items[i] , transform.position + randomization, Quaternion.identity);
        }

        rigidbody.useGravity = true;
        fracture.CauseFracture();

        GameObject fragmentParent = GameObject.Find($"{name}Fragments");

        // Set fragments to be on the ignore item collision layer.
        int fragmentCount = fragmentParent.transform.childCount;
        for (int i = 0; i < fragmentCount; i++)
        {
            fragmentParent.transform.GetChild(i).gameObject.layer = gameObject.layer;
        }

        DespawnObject despawner = fragmentParent.AddComponent<DespawnObject>();
        despawner.StartDespawn(despawnDelay, despawnTime);
    }

    private float GetCurrentSpeed()
    {
        float distanceMoved = 0f;
        float timePassed = 0f;

        for (int i = 1; i < speedAccuracy; i++)
        {
            distanceMoved += Vector3.Distance(positions[i - 1], positions[i]);
            timePassed += deltaTimes[i];
        }
        return distanceMoved / timePassed;
    }

    private IEnumerator SavePositions()
    {
        positions = new Vector3[speedAccuracy];
        deltaTimes = new float[speedAccuracy];

        int frameCount = 0;
        while (isHeld)
        {
            yield return new WaitForEndOfFrame();

            for (int i = frameCount - 1; i > 0; i--)
            {
                positions[i] = positions[i - 1];
                deltaTimes[i] = deltaTimes[i - 1];
            }

            positions[0] = transform.position;
            deltaTimes[0] = Time.deltaTime;
            
            // Fills in empty spots with the oldest entry.
            if (frameCount < speedAccuracy)
            {
                frameCount++;
                for (int i = frameCount; i < speedAccuracy; i++)
                {
                    positions[i] = positions[frameCount - 1];
                    deltaTimes[i] = deltaTimes[frameCount - 1];
                }
            }
        }
    }
}
