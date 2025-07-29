using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Fracture))]
public class BoxExploder : MonoBehaviour
{
    [SerializeField] private BoxItemsSO boxItems;
    [SerializeField] private float minimumSmashSpeed = 15f;
    [SerializeField] private float despawnDelay = 3f;
    [SerializeField] private float despawnTime = 0.5f;
    private Fracture fracture;
    private new Rigidbody rigidbody;
    private Vector3[] positions;
    private float[] deltaTimes;
    private int speedSampleCount = 5;

    void Awake()
    {
        fracture = GetComponent<Fracture>();
        rigidbody = GetComponent<Rigidbody>();

        positions = new Vector3[speedSampleCount];
        deltaTimes = new float[speedSampleCount];
        for (int i = 0; i < speedSampleCount; i++)
        {
            positions[i] = transform.position;
        }
    }

    void Update()
    {
        for (int i = speedSampleCount - 1; i > 0; i--)
        {
            positions[i] = positions[i - 1];
            deltaTimes[i] = deltaTimes[i - 1];
        }

        positions[0] = transform.position;
        deltaTimes[0] = Time.deltaTime;
    }

    public void HeldItemCollision(GameObject heldItem)
    {
        if (heldItem == gameObject && GetCurrentSpeed() > minimumSmashSpeed)
        {
            InitiateSmash();
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

        for (int i = 1; i < speedSampleCount; i++)
        {
            distanceMoved += Vector3.Distance(positions[i - 1], positions[i]);
            timePassed += deltaTimes[i];
        }
        return distanceMoved / timePassed;
    }
}