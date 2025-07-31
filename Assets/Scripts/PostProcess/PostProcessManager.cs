using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum PostProcess
{
    Neutral,
    Clean,
    Corrode,
}

public class PostProcessManager : MonoBehaviour
{
    [Tooltip("Profiles must match the PostProcessEffect enum!")]
    [SerializeField] private VolumeProfile[] profiles;
    [SerializeField] private float transitionTime = .25f;
    private Volume mainVolume;
    private Dictionary<PostProcess, VolumeProfile> profileMap;
    private VolumeProfile activeProfile;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        mainVolume = GetComponent<Volume>();

        profileMap = new Dictionary<PostProcess, VolumeProfile>();
        for (int i = 0; i < profiles.Length; i++)
        {
            profileMap[(PostProcess) i] = profiles[i];
        }

        activeProfile = CloneProfile(profiles[0]);
        mainVolume.profile = activeProfile;
    }

    public void ChangePostProcess(int effectNumber)
    {
        PostProcess effect = (PostProcess)effectNumber;

        if (!profileMap.TryGetValue(effect, out VolumeProfile targetProfile))
        {
            Debug.LogWarning($"No profile assigned for effect: {effect}");
            return;
        }

        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
        }

        transitionRoutine = StartCoroutine(TransitionProfiles(targetProfile));
    }

    // CloneProfile is used over Instantiate so that we don't overwrite the VolumeProfile while switching.
    private VolumeProfile CloneProfile(VolumeProfile original)
    {
        VolumeProfile clone = ScriptableObject.CreateInstance<VolumeProfile>();
        foreach (var component in original.components)
        {
            var componentClone = Instantiate(component);
            clone.components.Add(componentClone);
        }
        return clone;
    }

    private IEnumerator TransitionProfiles(VolumeProfile targetProfile)
    {
        VolumeProfile startProfile = CloneProfile(activeProfile);
        VolumeProfile endProfile = CloneProfile(targetProfile);

        float timer = 0f;
        while (timer < transitionTime)
        {
            InterpolateProfiles(startProfile, endProfile, activeProfile, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }

        activeProfile = endProfile;

        Destroy(startProfile);
        Destroy(endProfile);

        transitionRoutine = null;
    }

    private void InterpolateProfiles(VolumeProfile from, VolumeProfile to, VolumeProfile output, float interpRatio)
    {
        interpRatio = Mathf.Clamp01(interpRatio);

        foreach (var fromComponent in from.components)
        {
            var type = fromComponent.GetType();
            var toComponent = to.components.Find(c => c.GetType() == type);
            var outputComponent = output.components.Find(c => c.GetType() == type);

            if (toComponent == null || outputComponent == null)
            {
                continue;
            }

            for (int i = 0; i < fromComponent.parameters.Count; i++)
            {
                InterpParameter
                (
                    outputComponent.parameters[i],
                    fromComponent.parameters[i],
                    toComponent.parameters[i],
                    interpRatio
                );
            }
        }
    }

    private static void InterpParameter(VolumeParameter output, VolumeParameter from, VolumeParameter to, float interpRatio)
    {
        // The Interp function is non-public so use reflection to get around C#'s access restrictions.
        var method = output.GetType().GetMethod
        (
            "Interp", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
        );

        object[] parameters = new object[] 
        {
            from,
            to,
            interpRatio
        };

        method?.Invoke(output, parameters);
    }
}