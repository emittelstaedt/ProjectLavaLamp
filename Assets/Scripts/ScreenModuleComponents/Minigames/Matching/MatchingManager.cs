using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchingManager : MonoBehaviour
{
	[SerializeField] private VoidEventChannelSO startMatchingInteract;
    [SerializeField] private VoidEventChannelSO stopInteract;
	[SerializeField] private VoidEventChannelSO Terminal3SirenStop;
	[SerializeField] private GameObject offScreen;
	[SerializeField] private GameObject screenShape;
	[SerializeField] private GameObject shapeHolder;
	[SerializeField] private Sprite[] shapes;
	[SerializeField] private Color[] colors;
	[SerializeField] private GameObject[] buttons;
	[SerializeField] private GameObject[] roundIndicators;
	[SerializeField] private SoundType onWrongShape;
	[SerializeField] private SoundType Button1Sound;
	[SerializeField] private SoundType Button2Sound;
	[SerializeField] private SoundType Button3Sound;
	[SerializeField] private SoundType Button4Sound;
	private int currentRound;
	private int correctShapes;
	private LinkedList<int> shapeSequence;
	private int[] shapeIndices;
	private bool hasStartedMatching;
	private bool isAnimating;
	private IEnumerator shapeVisual;
	
    void Awake()
    {
		TurnScreenOff(true);
    }


	public void StartMatchingInteraction()
	{
		if (hasStartedMatching)
        {
            return;
        }

        hasStartedMatching = true;

        if (startMatchingInteract != null)
        {
            startMatchingInteract.RaiseEvent();
        }
		
		resetMatchingGame();
		StartCoroutine(matchingAnimation());
	}
	
	public void TurnScreenOff(bool isActive)
	{
		offScreen.SetActive(isActive);
	}
	
	private void TurnButtonsOff(bool setting)
	{
		for(int i = 0; i < buttons.Length; i++)
		{
			Collider buttonCollider = buttons[i].GetComponent<Collider>();
			buttonCollider.enabled = setting;
		}
	}
	
	public void resetMatchingGame()
	{
		for(int i = 0; i < 3; i++)
		{
			roundIndicators[i].GetComponent<SpriteRenderer>().color = Color.white;
		}
		indicateMatchingRound();
		currentRound = 0;
		correctShapes = 0;
		shapeIndices = new int[6];
		shapeSequence = new LinkedList<int>();
		shapeSequence.Clear();
        for (int i = 0; i < 6; i++)
		{
			int newIndex = Random.Range(0, 4);
			if(i != 0)
			{
				while(shapeIndices[i - 1] == newIndex)
				{
					newIndex = Random.Range(0, 4);
				}
			}
			shapeIndices[i] = newIndex;
			if(i <= 2)
			{
				shapeSequence.AddLast(shapeIndices[i]);
			}
		}
	}
	
	public void StopMatchingInteraction()
	{
		if (!hasStartedMatching)
        {
            return;
        }

        hasStartedMatching = false;


        if(stopInteract != null)
        {
            stopInteract.RaiseEvent();
        }
		AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.MinigameComplete, 1f, transform.position);
        TurnScreenOff(true); 
	}
	
	public void buttonActivation(int buttonID)
	{
		LinkedListNode<int> current;
		current = shapeSequence.First;
		for(int i = 0; i < correctShapes; i++)
		{
			current = current.Next;
		}
		if(current.Value == buttonID)
		{
			PlayCorrespondingSound(buttonID);
			correctShape(buttonID);
		}
		else
		{
			wrongShape(buttonID);
		}
	}
	
	private void correctShape(int buttonID)
	{
		cancelShapeAnimations();
		shapeVisual = shapeAnimation(buttonID);
		StartCoroutine(shapeVisual);
		correctShapes++;
		
		if(correctShapes == shapeSequence.Count)
		{
			currentRound++;
			indicateMatchingRound();
			if(currentRound == 3)
			{
				StartCoroutine(completeMatchingGame());
			}
			else
			{
				StartCoroutine(completeMatchingRound());
			}
		}
		
		
	}
	
	private void wrongShape(int buttonID)
	{
		//Plays Audio Here now, move this line if this is wrong
		AudioManager.Instance.PlaySound(MixerType.SFX, onWrongShape, 1f);

		cancelShapeAnimations();
		shapeVisual = shapeAnimation(buttonID);
		StartCoroutine(shapeVisual);
		StartCoroutine(failedMatchingRound());
	}
	
	private IEnumerator matchingAnimation()
	{
		TurnButtonsOff(false);
		SpriteRenderer shapeDisplay = screenShape.GetComponent<SpriteRenderer>();
		shapeDisplay.sprite = null;
		yield return new WaitForSeconds(0.5f);
		LinkedListNode<int> current = shapeSequence.First;
		while(current != null)
		{
			PlayCorrespondingSound(current.Value);
			shapeDisplay.sprite = shapes[current.Value];
			shapeDisplay.color = colors[current.Value];
			yield return new WaitForSeconds(0.66f);
			current = current.Next;
		}
		shapeDisplay.sprite = null;
		TurnButtonsOff(true);
	}
	
	private IEnumerator shapeAnimation(int index)
	{
		isAnimating = true;
		SpriteRenderer shapeDisplay = screenShape.GetComponent<SpriteRenderer>();
		shapeDisplay.sprite = shapes[index];
		shapeDisplay.color = colors[index];
		yield return new WaitForSeconds(1f);
		isAnimating = false;
		shapeDisplay.sprite = null;
	}
	
	private void cancelShapeAnimations()
	{
		if(shapeVisual != null)
		{
			StopCoroutine(shapeVisual);
		}
		SpriteRenderer shapeDisplay = screenShape.GetComponent<SpriteRenderer>();
		shapeDisplay.sprite = null;
		
	}

	private void indicateMatchingRound()
	{
		for(int i = 0; i < 3; i++)
		{
			if(i == currentRound)
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = colors[3];
			}
			if(i < currentRound)
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = colors[1];
			}
		}
	}
	
	private IEnumerator completeMatchingRound()
	{
		TurnButtonsOff(false);
		while(isAnimating == true)
		{
			yield return null;
		}
		SpriteRenderer holderDisplay = shapeHolder.GetComponent<SpriteRenderer>();
		holderDisplay.color = Color.white;
		yield return new WaitForSeconds(0.5f);
		holderDisplay.color = Color.black;
		correctShapes = 0;
		shapeSequence.AddLast(shapeIndices[currentRound + 2]);
		StartCoroutine(matchingAnimation());
	}
	
	private IEnumerator completeMatchingGame()
	{
		TurnButtonsOff(false);
		while(isAnimating == true)
		{
			yield return null;
		}
		roundIndicators[2].GetComponent<SpriteRenderer>().color = colors[1];
		SpriteRenderer holderDisplay = shapeHolder.GetComponent<SpriteRenderer>();
		holderDisplay.color = Color.white;
		yield return new WaitForSeconds(0.5f);
		holderDisplay.color = Color.black;
		TurnButtonsOff(true);
		Terminal3SirenStop.RaiseEvent();
		StopMatchingInteraction();
	}
	
	private IEnumerator failedMatchingRound()
	{
		TurnButtonsOff(false);
		for(int i = 0; i < 3; i++)
		{
			if(i == currentRound)
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = colors[0];
			}
			if(i < currentRound)
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = colors[1];
			}
		}
		while(isAnimating == true)
		{
			yield return null;
		}
		shapeSequence.Clear();
		resetMatchingGame();
		for(int i = 0; i < 3; i++)
		{
			if(i == 0)
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = colors[3];
			}
			else
			{
				roundIndicators[i].GetComponent<SpriteRenderer>().color = Color.white;
			}
		}
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(matchingAnimation());
	}

	private void PlayCorrespondingSound(int which)
	{
		switch(which){
				case 0:
					AudioManager.Instance.PlaySound(MixerType.SFX, Button1Sound, 0.1f);
					break;
				case 1:
					AudioManager.Instance.PlaySound(MixerType.SFX, Button2Sound, 0.1f);
					break;
				case 2:
					AudioManager.Instance.PlaySound(MixerType.SFX, Button3Sound, 0.1f);
					break;
				case 3:
					AudioManager.Instance.PlaySound(MixerType.SFX, Button4Sound, 0.1f);
					break;
		}
	}
}
