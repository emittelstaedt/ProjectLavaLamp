using UnityEngine;

public class BuildOrderEnforcer : MonoBehaviour
{
    [Tooltip("Update to this name when there are no placement nodes remaining. Blank keeps current name.")]
    [SerializeField] private string finishedName;
    [Tooltip("Enable this if the object must be attached to something else before you can attach things to it.")]
    [SerializeField] private bool mustBePlaced;
    [SerializeField] private VoidEventChannelSO buildFinished;
    private GameObject[] placementNodes;

    private void Awake()
    {
        PlacementTrigger[] placementPoints = GetComponentsInChildren<PlacementTrigger>(true);

        placementNodes = new GameObject[placementPoints.Length];
        for (int i = 0; i < placementPoints.Length; i++)
        {
            placementNodes[i] = placementPoints[i].transform.parent.gameObject;
        }

        if (mustBePlaced)
        {
            TogglePlacementNodes(false);
        }
    }

    private void OnDestroy()
    {
        TogglePlacementNodes(true);
    }

    public void CheckIfFinishedBuilding()
    {
        PlacementTrigger[] placementPoints = GetComponentsInChildren<PlacementTrigger>(true);

        int placementCount = 0;
        for (int i = 0; i < placementPoints.Length; i++)
        {
            if (!placementPoints[i].IsUpstreamPlacement)
            {
                placementCount++;
            }
        }

        if (placementCount == 0)
        {
            UpdateName(finishedName);

            if (buildFinished != null)
            {
                buildFinished.RaiseEvent();
            }
        }
    }

    public void SetBuildOrder(string finishedName, bool mustBePlaced)
    {
        this.finishedName = finishedName;
        this.mustBePlaced = mustBePlaced;
    }

    public (string, bool) GetBuildOrder()
    {
        return (finishedName, mustBePlaced);
    }

    private void UpdateName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            gameObject.name = newName;
        }
    }

    private void TogglePlacementNodes(bool isActive)
    {
        for (int i = 0; i < placementNodes.Length; i++)
        {
            if (placementNodes[i] != null)
            {
                placementNodes[i].SetActive(isActive);
            }
        }
    }
}