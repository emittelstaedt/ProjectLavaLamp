using UnityEngine;

public class PlacementPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private string requiredItem;
    [SerializeField] private GameObject placementLocation;
    [SerializeField] private InteractableSettingsSO Settings;
    private Transform placementNode;
    private Transform buildItem;
    private Outline outline;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;

    private void Awake()
    {
        placementNode = transform.parent;
        buildItem = placementNode.parent;

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = 5;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public float GetInteractDistance()
    {
        return Settings.InteractionDistance;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool CanInteract()
    {
        if (currentItemHeld != null)
        {
            return requiredItem.Equals(currentItemHeld.name);
        }

        return false;
    }

    public void StartInteract()
    {
        Transform placementTransform = placementLocation.transform;

        lastItemheld.transform.SetPositionAndRotation(placementTransform.position, placementTransform.rotation);
        lastItemheld.transform.localScale = placementTransform.lossyScale;
        lastItemheld.transform.SetParent(placementNode, true);

        // Combine meshes so that the outline will show for the entire new combined object.
        MeshFilter parentMeshFilter = buildItem.GetComponent<MeshFilter>();
        MeshFilter itemMeshFilter = lastItemheld.GetComponent<MeshFilter>();
        parentMeshFilter.sharedMesh = AddChildMeshToParent(parentMeshFilter, itemMeshFilter);

        // Remove the previous item while keeping its children, i.e. its collider and placement points.
        while (lastItemheld.transform.childCount > 0)
        {
            Transform child = lastItemheld.transform.GetChild(0);
            child.SetParent(buildItem, true);
        }
        Destroy(lastItemheld);

        // Stop interacting with this script since continuing to interact not needed.
        stopInteraction.RaiseEvent();
    }

    public void StopInteract()
    {
        Destroy(placementNode.gameObject);
    }

    public void StartHover()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        if (currentItemHeld != null)
        {
            lastItemheld = currentItemHeld;
        }
    }

    private Mesh AddChildMeshToParent(MeshFilter parent, MeshFilter child)
    {
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = parent.sharedMesh;
        combine[0].transform = Matrix4x4.identity;

        Matrix4x4 childToParent = parent.transform.worldToLocalMatrix * child.transform.localToWorldMatrix;
        combine[1].mesh = child.sharedMesh;
        combine[1].transform = childToParent;

        Mesh mesh = new();
        mesh.CombineMeshes(combine);

        return mesh;
    }
}