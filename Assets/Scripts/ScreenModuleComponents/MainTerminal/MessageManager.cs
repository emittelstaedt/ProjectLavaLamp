using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour, IScreen
{
    [SerializeField] private Canvas messageBoard;
    [SerializeField] private TextMeshProUGUI messageBody;
    [SerializeField] private List<string> messageBodies = new List<string>();
    [SerializeField] private List<Button> unreadButtons;
    [SerializeField] private Transform readParent;
    [SerializeField] private Color readColor = Color.gray;
    [SerializeField] private Color highlightColor = Color.cyan;
    private int currentMessageIndex = 0;

    private void Awake()
    {
        DeactivateScreen();
    }

    public void ActivateScreen()
    {
        messageBoard.enabled = true;
    }

    public void DeactivateScreen()
    {
        messageBoard.enabled = false;
    }

    public bool isActive()
    {
        if (!messageBoard.enabled)
        {
            return false;
        }

        return true;
    }

    public void NextMessage()
    {
        currentMessageIndex = Mathf.Clamp(currentMessageIndex, 0, messageBodies.Count);

        if (!isActive())
        {
            return;
        }

        if (currentMessageIndex >= messageBodies.Count)
        {
            messageBody.text = "No new messages!";
            ClearHighlight();
            return;
        }

        MarkAsRead(currentMessageIndex);
        DisplayMessage(currentMessageIndex);
        HighlightCurrent();
        currentMessageIndex++;
    }

    public void PreviousMessage()
    {
        if (!isActive())
        {
            return;
        }

        if (currentMessageIndex > 0)
        {
            currentMessageIndex--;
            HighlightCurrent();
            DisplayMessage(currentMessageIndex);
        }
    }

    private void DisplayMessage(int index)
    {
        if (index >= 0 && index < messageBodies.Count)
        {
            messageBody.text = messageBodies[index];
        }
        else
        {
            messageBody.text = "No more messages!";
            ClearHighlight();
        }
    }

    private void MarkAsRead(int index)
    {
        if (index < unreadButtons.Count)
        {
            Button button = unreadButtons[index];

            // Do nothing if already marked.
            if (button.transform.parent == readParent)
            {
                return;
            }

            // Change button appearance
            var buttonBackground = button.GetComponent<Image>();
            if (buttonBackground != null)
            {
                buttonBackground.color = readColor;
            }

            button.transform.SetParent(readParent, false);
        }
    }

    private void HighlightCurrent()
    {
        ClearHighlight();

        if (currentMessageIndex < unreadButtons.Count)
        {
            Image currentImg = unreadButtons[currentMessageIndex].GetComponent<Image>();
            if (currentImg != null)
            {
                currentImg.color = highlightColor;
            }
        }
    }

    private void ClearHighlight()
    {
        for (int i = 0; i < unreadButtons.Count; i++)
        {
            Image image = unreadButtons[i].GetComponent<Image>();
            if (image != null)
            {
                if (unreadButtons[i].transform.parent == readParent)
                {
                    image.color = readColor;
                }
            }
        }
    }
}
