using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InChute : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableSettingsSO settings;
    [SerializeField] private GameObject packagedBox;
    [SerializeField] private GameObject outBox;
    [SerializeField] private GameObject itemSpawn;
    [SerializeField] private float delay = 0.5f;
    private Material material;
    private Outline outline;
    private Queue<BoxItemsSO> boxItemsQueue;
    private Vector3 itemSpawnPosition;
    private Rigidbody outBoxRigidbody;
    private Mover doorMover;
    private PistonMover pistonMover;
    private bool isAnimating;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.color = Color.green;

        if (!TryGetComponent<Outline>(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false;
        outline.OutlineWidth = settings.OutlineWidth;
        outline.OutlineMode = Outline.Mode.OutlineVisible;

        boxItemsQueue = new();

        itemSpawnPosition = itemSpawn.transform.position;

        doorMover = GetComponentInChildren<Mover>();
        pistonMover = GetComponentInChildren<PistonMover>();

        outBox = Instantiate(outBox, itemSpawnPosition + Vector3.right * 100f, Quaternion.identity);
        outBoxRigidbody = outBox.GetComponent<Rigidbody>();
        outBoxRigidbody.useGravity = false;
    }

    public void EnqueueItems(BoxItemsSO items)
    {
        boxItemsQueue.Enqueue(items);
        if (boxItemsQueue.Count == 1)
        {
            material.color = Color.green;
        }
    }

    public float GetInteractDistance()
    {
        return settings.InteractionDistance;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool CanInteract()
    {
        return !isAnimating && (boxItemsQueue.Count > 0 || outBox != null);
    }

    public void StartInteract()
    {
        GiveBox();
    }

    public void StopInteract()
    {
    }

    public void StartHover()
    {
        outline.OutlineColor = settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }

    private void GiveBox()
    {
        if (boxItemsQueue.Count > 0)
        {
            GameObject newBox = Instantiate(packagedBox, itemSpawnPosition, Quaternion.identity);
            // Removes "(Clone)" from the name.
            newBox.name = packagedBox.name;
            newBox.GetComponent<BoxExploder>().BoxItems = boxItemsQueue.Dequeue();
        }
        else
        {
            outBox.transform.position = itemSpawnPosition;
            outBoxRigidbody.useGravity = true;
            outBox = null;
        }

        StartCoroutine(GiveItemAnimation());
    }

    private IEnumerator GiveItemAnimation()
    {
        isAnimating = true;

        yield return new WaitForSeconds(delay);

        doorMover.Move();

        yield return new WaitForSeconds(delay);

        pistonMover.StartPush();

        yield return new WaitForSeconds(delay);

        doorMover.MoveBack();

        if (boxItemsQueue.Count <= 0 && outBox == null)
        {
            material.color = Color.white;
        }

        isAnimating = false;
    }
}