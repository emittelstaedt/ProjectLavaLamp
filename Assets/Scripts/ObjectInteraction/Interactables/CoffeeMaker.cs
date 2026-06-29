using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoffeeMaker : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private string requiredItem = "CoffeeCupEmpty";
    [SerializeField] private GameObject fullCupPrefab;
    [SerializeField] private InteractableSettingsSO settings;
    [SerializeField] private float transitionTime = .5f;
    [SerializeField] private Transform coffeeLocationBottom;
    [SerializeField] private ParticleSystem coffeeSteam;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
	private GameObject emptyCup;
    private Outline outline;
    private bool isPlacingCoffee;

    private void Awake()
    {
		if (!TryGetComponent<Outline>(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false;
        outline.OutlineWidth = settings.OutlineWidth;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    public float GetInteractDistance()
    {
        return settings.InteractionDistance + 3;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool CanInteract()
    {
        if (currentItemHeld != null && !isPlacingCoffee)
        {
            return requiredItem.Equals(currentItemHeld.name);
        }

        return false;
    }

    public void StartInteract()
    {
        stopInteraction.RaiseEvent();

        Quaternion currentRotation = lastItemheld.transform.rotation;
        lastItemheld.transform.rotation = Quaternion.identity;

        float cupHeight = lastItemheld.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 cupOffset = new(0f, cupHeight, 0f);
        Vector3 coffeeLocation = coffeeLocationBottom.transform.position + cupOffset;

        lastItemheld.transform.rotation = currentRotation;

        isPlacingCoffee = true;
		emptyCup = lastItemheld;
        StartCoroutine(MoveCoffeeToMachine(emptyCup, coffeeLocation));

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

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        if (currentItemHeld != null)
        {
            lastItemheld = currentItemHeld;
        }
    }

    private IEnumerator MoveCoffeeToMachine(GameObject emptyCup, Vector3 location)
    {
        // Reduce empty cup to just mesh renderer.
        Component[] components = emptyCup.transform.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is not Transform && component is not MeshFilter && component is not MeshRenderer)
            {
                Destroy(component);
            }
        }
        while (emptyCup.transform.childCount > 0)
        {
            Destroy(emptyCup.transform.GetChild(0).gameObject);
            yield return null;
        }
 
        emptyCup.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        float timer = 0f;
        while (timer < transitionTime)
        {
            Vector3 newPosition = Vector3.Slerp(startPosition, location, timer / transitionTime);
            Quaternion newRotation = Quaternion.Slerp(startRotation, Quaternion.identity, timer / transitionTime);

            emptyCup.transform.SetPositionAndRotation(newPosition, newRotation);

            timer += Time.deltaTime;
            yield return null;
        }
        emptyCup.transform.SetPositionAndRotation(location, Quaternion.identity);

        coffeeSteam.Play();

        // Waits until the audio is done playing. Manual number since audio manager doesn't support this.
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.CoffeeBrew, 1f, transform.position);
        yield return new WaitForSeconds(2.5f);

        coffeeSteam.Stop();

        GameObject fullCup = Instantiate(fullCupPrefab, emptyCup.transform.position, emptyCup.transform.rotation);
        fullCup.transform.localScale = emptyCup.transform.localScale;
        fullCup.name = fullCupPrefab.name;
		SceneManager.MoveGameObjectToScene(fullCup, SceneManager.GetSceneByName("OfficeWorkplace"));
        Destroy(emptyCup);

        isPlacingCoffee = false;
    }
}