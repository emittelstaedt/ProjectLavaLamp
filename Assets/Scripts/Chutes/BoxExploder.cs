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
    private float halfBoxSize;
    private Vector3[] positions;
    private float[] deltaTimes;
    private int speedSampleCount = 5;

    void Awake()
    {
        fracture = GetComponent<Fracture>();
        rigidbody = GetComponent<Rigidbody>();

        halfBoxSize = GetComponentInChildren<BoxCollider>().size.x / 2;

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
        InstantiateBoxItems();

        rigidbody.useGravity = true;
        fracture.CauseFracture();

        // The object OpenFracture places the fragments under.
        GameObject fragmentParent = GameObject.Find($"{name}Fragments");

        // Set fragments to be on the ignore item collision layer.
        int fragmentCount = fragmentParent.transform.childCount;
        for (int i = 0; i < fragmentCount; i++)
        {
            fragmentParent.transform.GetChild(i).gameObject.layer = gameObject.layer;
        }

        DespawnObject despawner = fragmentParent.AddComponent<DespawnObject>();
        despawner.StartDespawn(despawnTime, despawnDelay);
    }

    private void InstantiateBoxItems()
    {
        if (boxItems != null)
        {
            GameObject[] items = boxItems.Items;
            for (int i = 0; i < items.Length; i++)
            {
                // Ensures objects fly in different directions.
                Vector3 randomization = new(Random.Range(-halfBoxSize, halfBoxSize),
                                            Random.Range(-halfBoxSize, halfBoxSize),
                                            Random.Range(-halfBoxSize, halfBoxSize));

                GameObject item = Instantiate(items[i] , transform.position + randomization, Quaternion.identity);
                // Removes "(Clone)" from the name.
                item.name = items[i].name;
            }
        }
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