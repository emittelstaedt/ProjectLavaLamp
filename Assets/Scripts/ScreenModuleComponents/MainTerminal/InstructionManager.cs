using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour, IScreen
{
    [SerializeField] private Canvas instructionCanvas;
    [SerializeField] private Image instructionsImage;
    [SerializeField] BuildInstructionsSO instructions;
    private int currentPage;

    public bool IsActive()
    {
        if (!instructionCanvas.enabled)
        {
            return false;
        }

        return true;
    }

    public void Awake()
    {
        DeactivateScreen();
    }

    public void ActivateScreen()
    {
        instructionCanvas.enabled = true;
    }

    public void DeactivateScreen()
    {
        instructionCanvas.enabled = false;
    }

    public void SetBuildInstructions(BuildInstructionsSO newInstructions)
    {
        instructions = newInstructions;
		instructionsImage.sprite = instructions.Pages[0];
    }

    public void NextPage()
    {
        if (!IsActive())
        {
            return;
        }

        if (currentPage < instructions.Pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (!IsActive())
        {
            return;
        }

        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int pageIndex)
    {
        instructionsImage.sprite = instructions.Pages[pageIndex];
    }
}