using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.VisualScripting;

public class KeyRebinding : MonoBehaviour
{   
    [SerializeField] private Button rebindButton;
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private string bindingName;
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private GameObject warningBackground;
    [SerializeField] private float delay = 2f;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private int bindingIndex;

    private void Start()
    {
        bindingIndex = GetBindingIndexByName();
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

        InputBinding binding = actionReference.action.bindings[bindingIndex];
        string displayString = "";
        
        if (binding.effectivePath != "<None>")
        {
            displayString = actionReference.action.GetBindingDisplayString(bindingIndex);
        }
        bindingText.text = displayString;
    }

    public void StartRebinding()
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
                foreach (InputActionMap map in actionReference.action.actionMap.asset.actionMaps)
                {
                    bool hasConflict = false;
                    foreach (InputAction action in map.actions)
                    {
                        if (action.bindings[0].isComposite)
                        {
                            foreach (InputBinding binding in action.bindings)
                            {
                                if (binding != action.bindings[bindingIndex] && !binding.isComposite)
                                {
                                    hasConflict = CheckForConflicts(action, newBindingPath);
                                    if (hasConflict)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (action == actionReference.action)
                        {
                            continue;
                        }

                        if (hasConflict)
                        {
                            break;
                        }

                        if (CheckForConflicts(action, newBindingPath))
                        {
                            break;
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
                InputSystem.actions.FindAction("Pause").Enable();
            })
            .Start();
    }

    private bool CheckForConflicts(InputAction action, string newBindingPath)
    {
        int conflictIndex = -1;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            bool isSameBinding = action.bindings[i].effectivePath == newBindingPath;
            bool isComposite = action.bindings[0].isComposite;
            if (isSameBinding && (!isComposite || (i != bindingIndex && isComposite)))
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
            foreach (KeyRebinding button in FindObjectsByType<KeyRebinding>(FindObjectsSortMode.None))
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
            return true;
        }
        else
        {
            if (warningText != null)
            {
                warningBackground.gameObject.SetActive(false);
            }
        }

        return false;
    }

    private IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (warningText != null)
        {
            warningBackground.gameObject.SetActive(false);
        }
    }

    private int GetBindingIndexByName()
    {
        InputBinding[] bindings = actionReference.action.bindings.ToArray();
        for (int i = 0; i < bindings.Length; i++)
        {
            if (String.Compare(bindings[i].name, bindingName, true) == 0 || String.IsNullOrEmpty(bindings[i].name))
            {
                return i;
            }
        }

        return -1;
    }

    private void OnDisable()
    {
        rebindingOperation?.Dispose();
    }
}