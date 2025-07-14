using UnityEngine;

public class PlacementPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private string requiredItem;
    [SerializeField] private GameObject placementLocation;
    [SerializeField] private InteractableSettingsSO Settings;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
    private Outline outline;

    private void Awake()
    {
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
        lastItemheld.transform.SetParent(transform.parent, true);

        // Current held item becomes only a renderer for its mesh. Collider stays so the parent rigidbody can use it.
        Component[] components = lastItemheld.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is not Transform &&
                component is not MeshFilter &&
                component is not MeshRenderer &&
                component is not Collider)
            {
                Destroy(component);
            }
        }  

        // Combine meshes so that the outline will show for the entire new combined object.
        MeshFilter parentMeshFilter = transform.parent.GetComponent<MeshFilter>();
        MeshFilter itemMeshFilter = lastItemheld.GetComponent<MeshFilter>();
        parentMeshFilter.sharedMesh = AddChildMeshToParent(parentMeshFilter, itemMeshFilter);

        // Stop interacting with this script since continuing to interact not needed.
        stopInteraction.RaiseEvent();
    }

    public void StopInteract()
    {
        Destroy(gameObject);
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