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
    private Material chuteMaterial;
    private Outline outline;
    private Queue<BoxItemsSO> boxItemsQueue;
    private Vector3 itemSpawnPosition;
    private Mover doorMover;
    private PistonMover pistonMover;
    private bool isAnimating;
    private bool hasGivenOutBox;
    private string outBoxRequiredItem;

    void Awake()
    {
        chuteMaterial = GetComponent<Renderer>().material;
        chuteMaterial.color = Color.green;

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
        return !isAnimating && (boxItemsQueue.Count > 0 || !hasGivenOutBox);
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

    public void SetOutBoxRequiredItem(string requiredItem)
    {
        outBoxRequiredItem = requiredItem;
    }

    public void AddBox(BoxItemsSO items)
    {
        boxItemsQueue.Enqueue(items);
        if (boxItemsQueue.Count == 1)
        {
            chuteMaterial.color = Color.green;
        }
    }

    private void GiveBox()
    {
        if (hasGivenOutBox)
        {
            GameObject newBox = Instantiate(packagedBox, itemSpawnPosition, Quaternion.identity);
            // Removes "(Clone)" from the name.
            newBox.name = packagedBox.name;
            newBox.GetComponent<BoxExploder>().BoxItems = boxItemsQueue.Dequeue();
        }
        else
        {
            GameObject newOutBox = Instantiate(outBox, itemSpawnPosition, Quaternion.identity);
            // Removes "(Clone)" from the name.
            newOutBox.name = outBox.name;

            newOutBox.GetComponentInChildren<PlacementTrigger>().SetRequiredItem(outBoxRequiredItem);

            hasGivenOutBox = true;
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

        if (boxItemsQueue.Count <= 0 && hasGivenOutBox)
        {
            chuteMaterial.color = Color.white;
        }

        isAnimating = false;
    }
}