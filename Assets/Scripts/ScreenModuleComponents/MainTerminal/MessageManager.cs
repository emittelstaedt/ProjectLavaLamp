using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Might incorporate a limitation to the number of emails to make some of this easier

public class MessageManager : MonoBehaviour, IScreen
{
    [SerializeField] private Canvas messageBoard;
    [SerializeField] private TextMeshProUGUI subject;
	[SerializeField] private TextMeshProUGUI handle;
	[SerializeField] private TextMeshProUGUI content;
	[SerializeField] private TextMeshProUGUI[] shortHands;
	[SerializeField] private Image[] shortHandHighlights;
    [SerializeField] private LinkedList<email> emailList = new LinkedList<email>();
    private int currentMessageIndex = 0;
	
	public void Awake()
	{
		StartCoroutine(InitialMessage());
	}
	
    public void ActivateScreen()
    {
        messageBoard.enabled = true;
    }

    public void DeactivateScreen()
    {
        messageBoard.enabled = false;
    }

    public bool IsActive()
    {
        if (!messageBoard.enabled)
        {
            return false;
        }

        return true;
    }

    public void NextMessage()
    {
        if (!IsActive())
        {
            return;
        }
		currentMessageIndex++;
		currentMessageIndex = Mathf.Clamp(currentMessageIndex, 0, emailList.Count - 1);
        DisplayMessage();
    }

    public void PreviousMessage()
    {
        if (!IsActive())
        {
            return;
        }
        currentMessageIndex--;
		currentMessageIndex = Mathf.Clamp(currentMessageIndex, 0, emailList.Count - 1);
        DisplayMessage();
    }

    private void DisplayMessage()
    {
		LinkedListNode<email> current = emailList.First;
		int positionIndex = 0;
		while(positionIndex != currentMessageIndex)
		{
			if(current != null)
			{
				current = current.Next;
				positionIndex++;
			}
			else
			{
				break;
			}
		}
		for(int i = 0; i < 6; i++)
		{
			shortHandHighlights[i].color = new Color(255f, 255f, 255f, 0f);
			shortHands[i].color = Color.white;
		}
		shortHandHighlights[positionIndex].color = new Color(255f, 255f, 255f, 255f);
		shortHands[positionIndex].color = Color.black;
		subject.text = current.Value.responseSubject;
		handle.text = current.Value.handle;
		content.text = current.Value.responseContent;
    }
	
	private void DisplayShortHands()
	{
		LinkedListNode<email> current = emailList.First;
		int positionIndex = 0;
		while(positionIndex != emailList.Count)
		{
			shortHands[positionIndex].text = current.Value.shortHand;
			current = current.Next;
			positionIndex++;
		}
	}
	
	public void AddMessage(email newMail)
	{
		emailList.AddLast(newMail);
	}
	
	private IEnumerator InitialMessage()
	{
		yield return new WaitForSeconds(1.5f);
		DisplayShortHands();
		DisplayMessage();
	}
}
