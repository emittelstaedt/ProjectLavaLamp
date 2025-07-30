using UnityEngine;

public class CoffeeMaker : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private string requiredItem = "CoffeeCupEmpty";
    [SerializeField] private GameObject fullCupPrefab;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private int numberOfUses = 1;
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
        if (currentItemHeld != null && numberOfUses > 0)
        {
            return requiredItem.Equals(currentItemHeld.name);
        }

        return false;
    }

    public void StartInteract()
    {
        stopInteraction.RaiseEvent();

        GameObject fullCup = Instantiate(fullCupPrefab, lastItemheld.transform.position, lastItemheld.transform.rotation);
        fullCup.transform.localScale = lastItemheld.transform.localScale;
        fullCup.name = fullCupPrefab.name;

        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.CoffeeBrew, 1f, transform.position);

        Destroy(lastItemheld);

        numberOfUses--;
    }

    public void StopInteract()
    {
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
}