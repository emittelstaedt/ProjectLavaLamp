using UnityEngine;
using System.Linq;

public class PlacementPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private VoidEventChannelSO updateName;
    [SerializeField] private string requiredItem;
    [SerializeField] private Transform placementLocation;
    [SerializeField] private InteractableSettingsSO Settings;
    private Transform placementNode;
    private Outline outline;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;

    private void Awake()
    {
        placementNode = transform.parent;

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
        lastItemheld.transform.SetPositionAndRotation(placementLocation.position, placementLocation.rotation);
        lastItemheld.transform.localScale = placementLocation.lossyScale;
        lastItemheld.transform.SetParent(placementNode, true);

        // Remove the previous item while keeping its children, i.e. its collider and placement points.
        while (lastItemheld.transform.childCount > 0)
        {
            Transform child = lastItemheld.transform.GetChild(0);
            child.SetParent(placementNode.parent, true);
        }
        Destroy(lastItemheld);

        // Combine meshes so that the outline will show for the entire new combined object.
        MeshFilter parentMeshFilter = placementNode.parent.GetComponent<MeshFilter>();
        MeshFilter itemMeshFilter = lastItemheld.GetComponent<MeshFilter>();

        parentMeshFilter.sharedMesh = AddChildMeshToParent(parentMeshFilter, itemMeshFilter);

        // Stop interacting with this script since continuing to interact not needed.
        stopInteraction.RaiseEvent();
    }

    public void StopInteract()
    {
        // Since destory is called at end of frame detach it so the renamer script doesn't see it.
        placementNode.SetParent(null);
        Destroy(placementNode.gameObject);

        updateName.RaiseEvent();
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
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}