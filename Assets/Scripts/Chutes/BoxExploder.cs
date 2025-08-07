using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Fracture))]
public class BoxExploder : MonoBehaviour
{
    [SerializeField] private float minimumSmashSpeed = 15f;
    [SerializeField] private float despawnDelay = 3f;
    [SerializeField] private float despawnTime = 0.5f;
    private Fracture fracture;
    private Rigidbody boxRigidbody;
    private Vector3[] positions;
    private float[] deltaTimes;
    private readonly int speedSampleCount = 5;
    private BoxItemsSO boxItems;

    public BoxItemsSO BoxItems
    {
        set => boxItems = value;
    }

    void Awake()
    {
        fracture = GetComponent<Fracture>();
        boxRigidbody = GetComponent<Rigidbody>();

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

        boxRigidbody.useGravity = true;
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

        // Prevents other boxes with the same name from trying to despawn these fragments.
        fragmentParent.name = $"{name}DespawningFragments";
    }

    private void InstantiateBoxItems()
    {
        if (boxItems != null)
        {
            BoxItem[] items = boxItems.Items;
            float distanceFromCenter = 1 - boxItems.Explosiveness;
            for (int i = 0; i < items.Length; i++)
            {
                for (int j = 0; j < items[i].count; j++)
                {
                    // Ensures objects fly in different directions.
                    Vector3 randomization = new(Random.Range(-distanceFromCenter, distanceFromCenter),
                                                Random.Range(-distanceFromCenter, distanceFromCenter),
                                                Random.Range(-distanceFromCenter, distanceFromCenter));
                    Vector3 position = transform.position + randomization;

                    GameObject item = Instantiate(items[i].gameObject, position, Quaternion.identity);
                    // Removes "(Clone)" from the name.
                    item.name = items[i].gameObject.name;
                }
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