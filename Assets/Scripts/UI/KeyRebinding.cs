using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class KeyRebinding : MonoBehaviour
{   
    [SerializeField] private Button rebindButton;
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private int bindingIndex;
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private GameObject warningBackground;
    [SerializeField] private float delay = 2f;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(StartRebinding);
        }

        string savedRebinds = PlayerPrefs.GetString(actionReference.action.name + "_rebinds", null);
        if (!string.IsNullOrEmpty(savedRebinds))
        {
            actionReference.action.LoadBindingOverridesFromJson(savedRebinds);
        }

        UpdateBindingDisplay();
    }

    private void UpdateBindingDisplay()
    {
        if (actionReference == null || bindingText == null)
        {
            return;
        }

        var binding = actionReference.action.bindings[bindingIndex];
        string displayString = "";
        
        if (binding.effectivePath != "<None>")
        {
            displayString = actionReference.action.GetBindingDisplayString(bindingIndex);
        }
        bindingText.text = displayString;
    }

    private void StartRebinding()
    {
        if (actionReference == null)
        {
            return;
        }

        bindingText.text = "Press any key";

        actionReference.action.Disable();

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                string newBindingPath = actionReference.action.bindings[bindingIndex].effectivePath;

                // Checks for conflicting keybindings in all actions except the current one.
                foreach (var map in actionReference.action.actionMap.asset.actionMaps)
                {
                    foreach (var action in map.actions)
                    {
                        if (action == actionReference.action)
                            continue;

                        int conflictIndex = -1;
                        for (int i = 0; i < action.bindings.Count; i++)
                        {
                            if (action.bindings[i].effectivePath == newBindingPath)
                            {
                                conflictIndex = i;
                                break;
                            }
                        }

                        if (conflictIndex >= 0)
                        {
                            action.ApplyBindingOverride(conflictIndex, "<None>");

                            PlayerPrefs.SetString(action.name + "_rebinds", action.SaveBindingOverridesAsJson());
                            
                            // Resets the text for all the keybindings.
                            foreach (var button in FindObjectsByType<KeyRebinding>(FindObjectsSortMode.None))
                            {
                                if (button.actionReference.action == action && button.bindingIndex == conflictIndex)
                                {
                                    button.UpdateBindingDisplay();
                                    break;
                                }
                            }

                            if (warningText != null)
                            {
                                warningText.text = $"'{action.name}' was unbound due to duplicate keybindings.";
                                warningBackground.gameObject.SetActive(true);

                                StopAllCoroutines();
                                StartCoroutine(HideWarningAfterDelay(delay));
                            }
                            break;
                        }
                        else
                        {
                            if (warningText != null)
                            {
                                warningBackground.gameObject.SetActive(false);
                            }
                        }
                    }
                }

                actionReference.action.Enable();
                operation.Dispose();
                UpdateBindingDisplay();

                string rebinds = actionReference.action.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString(actionReference.action.name + "_rebinds", rebinds);
                PlayerPrefs.Save();
                InputSystem.actions.FindActionMap("Player").Disable();
            })
            .Start();
    }

    IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (warningText != null)
        {
            warningBackground.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        rebindingOperation?.Dispose();
    }
}